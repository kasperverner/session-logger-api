using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SessionLogger.Users;

namespace SessionLogger.Filters.Parameters;

public class UserIdFromRouteFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var userId = context.HttpContext.GetRouteValue("userId");

        if (userId is null || !Guid.TryParse(userId.ToString(), out var parsedUserId))
            return await next(context);
        
        var classes = typeof(IHaveUserId).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsInterface: false, IsAbstract: false } &&
                typeof(IHaveUserId).IsAssignableFrom(type))
            .ToList();
            
        foreach (var @class in classes)
        {
            var parameter = context.Arguments
                .FirstOrDefault(arg => arg?.GetType().IsAssignableTo(@class) ?? false);
            
            if (parameter is null) continue;
            
            var property = @class.GetProperty("UserId");
            property?.SetValue(parameter, parsedUserId);
        }

        return await next(context);
    }
}