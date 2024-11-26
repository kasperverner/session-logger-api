using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SessionLogger.Sessions;

namespace SessionLogger.Filters.Parameters;

public class SessionIdFromRouteFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var sessionId = context.HttpContext.GetRouteValue("sessionId");

        if (sessionId is null || !Guid.TryParse(sessionId.ToString(), out var parsedSessionId))
            return await next(context);
        
        var classes = typeof(IHaveSessionId).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsInterface: false, IsAbstract: false } &&
                typeof(IHaveSessionId).IsAssignableFrom(type))
            .ToList();
            
        foreach (var @class in classes)
        {
            var parameter = context.Arguments
                .FirstOrDefault(arg => arg?.GetType().IsAssignableTo(@class) ?? false);
            
            if (parameter is null) continue;
            
            var property = @class.GetProperty("SessionId");
            property?.SetValue(parameter, parsedSessionId);
        }

        return await next(context);
    }
}