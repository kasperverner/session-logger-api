using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Sessions;

// TODO: Modify

public class GetSessions : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get all sessions")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<CreateSessionRequest>()
            .WithResponse<IEnumerable<SessionResponse>>();
    
    public class RequestValidator : AbstractValidator<GetSessionsRequest>
    {
        public RequestValidator(ICustomerService customerService, IProjectService projectService, ITaskService taskService, IUserService userService)
        {
            When(x => x.CustomerIds.Length > 0, () => RuleForEach(x => x.CustomerIds)
                .MustAsync(async (id, ct) => await customerService.CustomerExistsAsync(id, ct))
                .WithMessage("Customer does not exist"));

            When(x => x.ProjectIds.Length > 0, () => RuleForEach(x => x.ProjectIds)
                .MustAsync(async (request, id, ct) => await projectService.ProjectExistsAsync(request.CustomerIds, id, ct))
                .WithMessage("Project does not exist"));
            
            When(x => x.TaskIds.Length > 0, () => RuleForEach(x => x.TaskIds)
                .MustAsync(async (request, id, ct) => await taskService.TaskExistsAsync(request.ProjectIds, id, ct))
                .WithMessage("Task does not exist"));
            
            When(x => x.UserIds.Length > 0, () => RuleForEach(x => x.UserIds)
                .MustAsync(async (id, ct) => await userService.UserExistsAsync(id, ct))
                .WithMessage("User does not exist"));
            
            When(x => x.EndDate.HasValue, () => RuleFor(x => x.EndDate)
                .Must((request, endDate) => !request.StartDate.HasValue || endDate > request.StartDate)
                .WithMessage("End date must come after start date"));
        }
    }
    
    private static async Task<Ok<IEnumerable<SessionResponse>>> Handle(
        [AsParameters] GetSessionsRequest request,
        [FromServices] ISessionService sessionService,
        CancellationToken ct)
    {
        var sessions = await sessionService.GetSessionsAsync(request, ct);
        
        return TypedResults.Ok(sessions);
    }
}