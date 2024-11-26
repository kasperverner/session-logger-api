using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SessionLogger.Departments;

namespace SessionLogger.Filters.Parameters;

public class DepartmentIdFromRouteFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var departmentId = context.HttpContext.GetRouteValue("departmentId");

        if (departmentId is null || !Guid.TryParse(departmentId.ToString(), out var parsedDepartmentId))
            return await next(context);
        
        var classes = typeof(IHaveDepartmentId).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsInterface: false, IsAbstract: false } &&
                typeof(IHaveDepartmentId).IsAssignableFrom(type))
            .ToList();
            
        foreach (var @class in classes)
        {
            var parameter = context.Arguments
                .FirstOrDefault(arg => arg?.GetType().IsAssignableTo(@class) ?? false);
            
            if (parameter is null) continue;
            
            var property = @class.GetProperty("DepartmentId");
            property?.SetValue(parameter, parsedDepartmentId);
        }

        return await next(context);
    }
}