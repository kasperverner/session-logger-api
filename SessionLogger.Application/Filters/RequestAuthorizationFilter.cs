using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SessionLogger.Exceptions;
using SessionLogger.Extensions;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Filters;

public static class RequestAuthorizationFilter
{
    /// <summary>
    /// Adds a filter to the route handler that requires the user to have the specified roles
    /// - If the user does not have the required roles, a ForbiddenAccessException is thrown
    /// - If the user does not exist, a new user is created with the principal id, name and email
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="roles"></param>
    /// <returns></returns>
    /// <exception cref="ForbiddenAccessException"></exception>
    /// <exception cref="ProblemException"></exception>
    public static RouteHandlerBuilder WithRequiredRoles(this RouteHandlerBuilder builder, params Role[] roles)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var userServices = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
            var cancellationToken = context.HttpContext.RequestAborted;

            try
            {
                var user = await userServices.GetAuthorizedUserAsync(cancellationToken);

                foreach (var role in roles)
                    if (!user.Roles.HasFlag(role))
                        throw new ForbiddenAccessException($"User does not have the required role \"{role.ToString()}\"");
                
            }
            catch (NotFoundException)
            {
                var principalId = context.HttpContext.User.GetPrincipalId();
                var name = context.HttpContext.User.GetPrincipalName();
                var email = context.HttpContext.User.GetPrincipalEmail();
                
                await userServices.CreateUserAsync(new CreateUserRequest(principalId, name, email), cancellationToken);
                
                if (roles.Length > 0)
                    throw new ForbiddenAccessException("User does not have any roles assigned - Contact a Manager to assign roles.");
            }

            return await next(context);
        });
    }
}