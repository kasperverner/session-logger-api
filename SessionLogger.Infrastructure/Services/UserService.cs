using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SessionLogger.Exceptions;
using SessionLogger.Extensions;
using SessionLogger.Interfaces;
using SessionLogger.Persistence;
using SessionLogger.Users;

namespace SessionLogger.Infrastructure.Services;

public class UserService(ILogger<UserService> logger, SessionLoggerContext context, IAuthorizationService authorizationService, IMemoryCache cache) : IUserService
{
    private const string UserCacheKeyPrefix = "User_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public Task<bool> UserExistsAsync(Guid userId, CancellationToken ct)
    {
        return context.Users.AnyAsync(x => x.Id == userId, ct);
    }
    
    public Task<bool> UserExistsAsync(Guid userId, Role roles, CancellationToken ct)
    {
        return context.Users.AnyAsync(x => x.Id == userId && x.Roles.HasFlag(roles), ct);
    }
    
    public async Task<IEnumerable<UserResponse>> GetUsersAsync(CancellationToken ct)
    {
        var authenticatedUser = await GetAuthorizedUserAsync(ct);

        if (!authenticatedUser.Roles.HasFlag(Role.Manager))
            return [ authenticatedUser ];
        
        var users = await context.Users
            .AsNoTracking()
            .Select(x => new UserResponse(x.Id, x.Name, x.Email, x.Roles))
            .ToListAsync(ct);
        
        return users;
    }
    
    public async Task<UserResponse> GetUserAsync(Guid userId, CancellationToken ct)
    {
        var authenticatedUser = await GetAuthorizedUserAsync(ct);
        
        var user = await context.Users
            .AsNoTracking()
            .Select(x => new UserResponse(x.Id, x.Name, x.Email, x.Roles))
            .FirstOrDefaultAsync(x => x.Id == userId, ct);
        
        if (!authenticatedUser.Roles.HasFlag(Role.Manager) && authenticatedUser.Id != userId)
            throw new ForbiddenAccessException("You do not have permission to view this user");
        
        return user ?? throw new NotFoundException(nameof(User), userId);
    }
    
    public async Task<UserResponse> GetAuthorizedUserAsync(CancellationToken ct)
    {
        var principalId = authorizationService.GetAuthorizedUserPrincipalId();
        
        var cacheKey = $"{UserCacheKeyPrefix}{principalId}";
        
        try
        {
            var userResponse = await cache.GetOrCreateAsync<UserResponse>(cacheKey, async entry =>
            {
                entry.SetSlidingExpiration(CacheDuration);
                
                var user = await context.Users
                    .AsNoTracking()
                    .Where(x => x.PrincipalId == principalId)
                    .Select(x => new UserResponse(x.Id, x.Name, x.Email, x.Roles))
                    .FirstOrDefaultAsync(ct);

                if (user is null)
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                
                return user ?? throw new NotFoundException(nameof(User), principalId);
            });

            return userResponse!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user for principalId: {PrincipalId}", principalId);
            cache.Remove(cacheKey);
            throw;
        }
    }
    
    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken ct)
    {
        var user = new User(request.PrincipalId, request.Name, request.Email);
        user.AssignRoles(request.Roles);
        
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
        
        logger.LogInformation("Created user {UserId}/{UserName}", user.Id, user.Name);
        
        return new UserResponse(user.Id, user.Name, user.Email, user.Roles);
    }
    
    public async Task UpdateUserAsync(UpdateUserRequest request, CancellationToken ct)
    {
        var user = await context.Users.FindAsync([request.Id], ct);

        if (user is null)
            throw new NotFoundException(nameof(User), request.Id);
        
        user.AssignRoles(request.Roles);
        
        logger.LogInformation("Updated roles for user {UserId}/{UserName}", user.Id, user.Name);
        
        context.Users.Update(user);
        await context.SaveChangesAsync(ct);
    }
    
    public async Task DeleteUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await context.Users.FindAsync([userId], ct);
        
        if (user is null)
            throw new NotFoundException(nameof(User), userId);
        
        user.Delete();
        
        logger.LogInformation("Deleted user {UserId}/{UserName}", user.Id, user.Name);
        
        context.Users.Update(user);
        await context.SaveChangesAsync(ct);
    }

    public Task<bool> UserHasActiveOptOUtAsync(Guid optOutId, CancellationToken ct)
    {
        return context.OptOuts.AnyAsync(x => x.UserId == optOutId && !x.Period.EndDate.HasValue, ct);
    }
    
    public Task<bool> OptOutExistsAsync(Guid optOutId, CancellationToken ct)
    {
        return context.OptOuts.AnyAsync(x => x.Id == optOutId, ct);    
    }
    
    public Task<bool> OptOutStartsBeforeAsync(Guid optOutGuid, DateTime endDate, CancellationToken ct)
    {
        return context.OptOuts.AnyAsync(x => x.Id == optOutGuid && x.Period.StartDate < endDate, ct);
    }
    
    public async Task<IEnumerable<OptOutResponse>> GetUserOptOutsAsync(Guid userId, CancellationToken ct)
    {
        var authenticatedUser = await GetAuthorizedUserAsync(ct);
        
        if (!authenticatedUser.Roles.HasFlag(Role.Manager) && authenticatedUser.Id != userId)
            throw new ForbiddenAccessException("You do not have permission to view opt outs for this user");
        
        var optOuts = await context.OptOuts
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new OptOutResponse(x.Id, x.UserId, x.Period, !x.Period.EndDate.HasValue))
            .OrderByDescending(x => x.Period.StartDate)
            .ToListAsync(ct);
        
        return optOuts;
    }

    public async Task<OptOutResponse> CreateOptOutAsync(CreateUserOptOutRequest request, CancellationToken ct)
    {
        var authenticatedUser = await GetAuthorizedUserAsync(ct);
        
        if (!authenticatedUser.Roles.HasFlag(Role.Manager) && authenticatedUser.Id != request.UserId)
            throw new ForbiddenAccessException("You do not have permission to view opt outs for this user");
        
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Id == request.UserId, ct);
        
        if (user is null)
            throw new NotFoundException(nameof(User), request.UserId);  
        
        var optOut = user.AddOptOut(request.StartDate, request.EndDate);
        
        logger.LogInformation("Created opt out {OptOutId} for user {UserId}", optOut.Id, user.Id);
        
        context.Update(user);
        await context.SaveChangesAsync(ct);
        
        return new OptOutResponse(optOut.Id, optOut.UserId, optOut.Period, !optOut.Period.EndDate.HasValue);
    }

    public async Task UpdateOptOutAsync(UpdateUserOptOutRequest request, CancellationToken ct)
    {
        var optOut = await context.OptOuts
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        
        if (optOut is null)
            throw new NotFoundException(nameof(OptOut), request.Id);
        
        var authenticatedUser = await GetAuthorizedUserAsync(ct);
        
        if (!authenticatedUser.Roles.HasFlag(Role.Manager) && authenticatedUser.Id != optOut.UserId)
            throw new ForbiddenAccessException("You do not have permission to update opt outs for this user");
        
        optOut.EndOptOut(request.EndDate);
        
        logger.LogInformation("Updated opt out {OptOutId} for user {UserId}", optOut.Id, optOut.UserId);
        
        context.Update(optOut);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteOptOutAsync(Guid outOutId, CancellationToken ct)
    {
        var optOut = await context.OptOuts
            .FirstOrDefaultAsync(x => x.Id == outOutId, ct);
        
        if (optOut is null)
            throw new NotFoundException(nameof(OptOut), outOutId);
        
        var authenticatedUser = await GetAuthorizedUserAsync(ct);
        
        if (!authenticatedUser.Roles.HasFlag(Role.Manager) && authenticatedUser.Id != optOut.UserId)
            throw new ForbiddenAccessException("You do not have permission to delete opt outs for this user");
        
        optOut.Delete();
        
        logger.LogInformation("Deleted opt out {OptOutId} for user {UserId}", optOut.Id, optOut.UserId);
        
        context.Update(optOut);
        await context.SaveChangesAsync(ct);
    }
}