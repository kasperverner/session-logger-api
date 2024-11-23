using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SessionLogger.Exceptions;
using SessionLogger.Interfaces;
using SessionLogger.Persistence;
using SessionLogger.Sessions;
using SessionLogger.Users;

namespace SessionLogger.Infrastructure.Services;

public class SessionService(ILogger<SessionService> logger, SessionLoggerContext context, IUserService userService) : ISessionService
{
    public async Task<bool> SessionExistsAsync(Guid sessionId, CancellationToken ct)
    {
        return await context.Sessions.AnyAsync(s => s.Id == sessionId && !s.Period.EndDate.HasValue, ct);
    }
    
    public async Task<bool> SessionExistsAsync(Guid sessionId, SessionState state, CancellationToken ct)
    {
        return await context.Sessions.OfType<ProjectSession>().AnyAsync(s => s.Id == sessionId && s.State == state, ct);
    }

    public async Task<IEnumerable<SessionResponse>> GetSessionsAsync(GetSessionsRequest request, CancellationToken ct)
    {
        var sessionsQuery = context.Sessions
            .AsTracking();
        
        if (request.CustomerIds.Length > 0)
            sessionsQuery = sessionsQuery
                .OfType<ProjectSession>()
                .Where(x => request.CustomerIds.Contains(x.Task.Project.CustomerId));;
        
        if (request.ProjectIds.Length > 0)
            sessionsQuery = sessionsQuery
                .OfType<ProjectSession>()
                .Where(x => request.ProjectIds.Contains(x.Task.ProjectId));
        
        if (request.TaskIds.Length > 0)
            sessionsQuery = sessionsQuery
                .OfType<ProjectSession>()
                .Where(x => request.TaskIds.Contains(x.TaskId));

        if (request.UserIds.Length > 0)
        {
            var authenticatedUser = await userService.GetAuthorizedUserAsync(ct);
        
            if (request.UserIds.Any(id => id != authenticatedUser.Id) && !authenticatedUser.Roles.HasFlag(Role.Manager))
                throw new ForbiddenAccessException("User is not authorized to view the sessions for the selected users.");
            
            sessionsQuery = sessionsQuery
                .Where(x => request.UserIds.Contains(x.UserId));
        }
            
        
        if (request.StartDate.HasValue)
            sessionsQuery = sessionsQuery
                .Where(x => x.Period.StartDate >= request.StartDate);
        
        if (request.EndDate.HasValue)
            sessionsQuery = sessionsQuery
                .Where(x => x.Period.EndDate <= request.EndDate);
        
        var sessionsResponseQuery = await GetSessionsResponseQuery(sessionsQuery, ct);
        
        var sessions = await sessionsResponseQuery.ToListAsync(ct);
        
        return sessions;
    }

    public async Task<SessionResponse> GetSessionAsync(Guid sessionId, CancellationToken ct)
    {
        var sessionsQuery = context.Sessions
            .AsTracking()
            .Where(x => x.Id == sessionId);
        
        var sessionsResponseQuery = await GetSessionsResponseQuery(sessionsQuery, ct);
     
        var session = await sessionsResponseQuery.FirstOrDefaultAsync(ct);
        
        if (session is null)
            throw new NotFoundException(nameof(Session), sessionId);
        
        var authenticatedUser = await userService.GetAuthorizedUserAsync(ct);
        
        if (session.UserId != authenticatedUser.Id && !authenticatedUser.Roles.HasFlag(Role.Manager))
            throw new ForbiddenAccessException("User is not authorized to view the session.");
        
        return session;
    }
    
    private async Task<IQueryable<SessionResponse>> GetSessionsResponseQuery(IQueryable<Session> sessionsQuery, CancellationToken ct)
    {
        var projectSessionsQuery = sessionsQuery
            .OfType<ProjectSession>()
            .Include(s => s.Task)
            .Select(x => new SessionResponse(x.Id, x.UserId, x.Type, x.Period, x.Description, x.State, 
                new SessionTask(x.Task.Id, x.Task.Name, x.Task.Description)));

        var checkInSessionsQuery = sessionsQuery
            .OfType<CheckInSession>()
            .Select(x => new SessionResponse(x.Id, x.UserId, x.Type, x.Period, null, null, null));

        return projectSessionsQuery.Union(checkInSessionsQuery);
    }

    public async Task<SessionResponse> CreateProjectSessionAsync(CreateSessionRequest request, CancellationToken ct)
    {
        var authenticatedUser = await userService.GetAuthorizedUserAsync(ct);

        if (await UserHasActiveSessionAsync(authenticatedUser.Id, ct))
            throw new ProblemException("User has an active session", "User already has an active session.");
        
        var task = await context.Tasks.FirstOrDefaultAsync(x => x.Id == request.TaskId, ct);
        
        if (task is null)
            throw new NotFoundException(nameof(Tasks.Task), request.TaskId);
        
        var session = new ProjectSession(authenticatedUser.Id, task, request.Description, request.StartDate, request.EndDate);
        
        await context.Sessions.AddAsync(session, ct);
        await context.SaveChangesAsync(ct);
        
        return new SessionResponse(session.Id, session.UserId, session.Type, session.Period, session.Description, session.State, 
            new SessionTask(task.Id, task.Name, task.Description));
    }

    public async Task UpdateSessionAsync(UpdateSessionRequest request, CancellationToken ct)
    {
        var session = await context.Sessions
            .OfType<ProjectSession>()
            .Where(x => x.Id == request.SessionId)
            .FirstOrDefaultAsync(ct);
        
        if (session is null)
            throw new NotFoundException(nameof(ProjectService), request.SessionId);
        
        if (session.State == SessionState.Billed)
            throw new ProblemException("Session is billed", "Session cannot be updated because it has been billed.");
        
        var authenticatedUser = await userService.GetAuthorizedUserAsync(ct);
        
        if (!authenticatedUser.Roles.HasFlag(Role.Manager) && authenticatedUser.Id != session.UserId)
            throw new ForbiddenAccessException("User is not authorized to delete the session.");
        
        session.UpdateDescription(request.Description);
        
        if (request.State.HasValue)
            session.UpdateState(request.State.Value);
        
        if (request.StartDate.HasValue)
            session.UpdatePeriod(request.StartDate.Value, session.Period.EndDate);
        
        if (request.EndDate.HasValue)
            session.Period.EndPeriod(request.EndDate.Value);
        
        if (session is { State: SessionState.Open, Period.EndDate: not null })
            session.UpdateState(SessionState.Closed);
        
        logger.LogInformation("Deleted session {SessionId}", session.Id);
        
        context.Sessions.Update(session);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteSessionAsync(Guid sessionId, CancellationToken ct)
    {
        var session = await context.Sessions
            .OfType<ProjectSession>()
            .Where(x => x.Id == sessionId)
            .FirstOrDefaultAsync(ct);
        
        if (session is null)
            throw new NotFoundException(nameof(ProjectService), sessionId);
        
        if (session.State == SessionState.Billed)
            throw new ProblemException("Session is billed", "Session cannot be deleted because it has been billed.");
        
        var authenticatedUser = await userService.GetAuthorizedUserAsync(ct);
        
        if (!authenticatedUser.Roles.HasFlag(Role.Manager) && authenticatedUser.Id != session.UserId)
            throw new ForbiddenAccessException("User is not authorized to delete the session.");
        
        session.Delete();
        
        logger.LogInformation("Deleted session {SessionId}", session.Id);
        
        context.Sessions.Update(session);
        await context.SaveChangesAsync(ct);
    }
    
    private async Task<bool> UserHasActiveSessionAsync(Guid userId, CancellationToken ct)
    {
        return await context.Sessions.AnyAsync(s => s.UserId == userId && !s.Period.EndDate.HasValue, ct);
    }
}