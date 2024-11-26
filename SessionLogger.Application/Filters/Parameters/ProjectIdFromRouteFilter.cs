using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SessionLogger.Projects;

namespace SessionLogger.Filters.Parameters;

public class ProjectIdFromRouteFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var projectId = context.HttpContext.GetRouteValue("projectId");

        if (projectId is null || !Guid.TryParse(projectId.ToString(), out var parsedProjectId))
            return await next(context);
        
        var classes = typeof(IHaveProjectId).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsInterface: false, IsAbstract: false } &&
                typeof(IHaveProjectId).IsAssignableFrom(type))
            .ToList();
            
        foreach (var @class in classes)
        {
            var parameter = context.Arguments
                .FirstOrDefault(arg => arg?.GetType().IsAssignableTo(@class) ?? false);
            
            if (parameter is null) continue;
            
            var property = @class.GetProperty("ProjectId");
            property?.SetValue(parameter, parsedProjectId);
        }

        return await next(context);
    }
}