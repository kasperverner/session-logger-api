using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SessionLogger.Tasks;

namespace SessionLogger.Filters.Parameters;

public class TaskIdFromRouteFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var taskId = context.HttpContext.GetRouteValue("taskId");

        if (taskId is null || !Guid.TryParse(taskId.ToString(), out var parsedTaskId))
            return await next(context);
        
        var classes = typeof(IHaveTaskId).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsInterface: false, IsAbstract: false } &&
                typeof(IHaveTaskId).IsAssignableFrom(type))
            .ToList();
            
        foreach (var @class in classes)
        {
            var parameter = context.Arguments
                .FirstOrDefault(arg => arg?.GetType().IsAssignableTo(@class) ?? false);
            
            if (parameter is null) continue;
            
            var property = @class.GetProperty("TaskId");
            property?.SetValue(parameter, parsedTaskId);
        }

        return await next(context);
    }
}