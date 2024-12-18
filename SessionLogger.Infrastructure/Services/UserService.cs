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
}