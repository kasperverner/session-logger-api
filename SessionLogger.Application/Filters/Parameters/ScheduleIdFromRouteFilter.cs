using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SessionLogger.Schedules;

namespace SessionLogger.Filters.Parameters;

public class ScheduleIdFromRouteFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var scheduleId = context.HttpContext.GetRouteValue("scheduleId");

        if (scheduleId is null || !Guid.TryParse(scheduleId.ToString(), out var parsedScheduleId))
            return await next(context);
        
        var classes = typeof(IHaveScheduleId).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsInterface: false, IsAbstract: false } &&
                typeof(IHaveScheduleId).IsAssignableFrom(type))
            .ToList();
            
        foreach (var @class in classes)
        {
            var parameter = context.Arguments
                .FirstOrDefault(arg => arg?.GetType().IsAssignableTo(@class) ?? false);
            
            if (parameter is null) continue;
            
            var property = @class.GetProperty("ScheduleId");
            property?.SetValue(parameter, parsedScheduleId);
        }

        return await next(context);
    }
}