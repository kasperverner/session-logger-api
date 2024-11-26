using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SessionLogger.Customers;

namespace SessionLogger.Filters.Parameters;

public class CustomerIdFromRouteFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var customerId = context.HttpContext.GetRouteValue("customerId");

        if (customerId is null || !Guid.TryParse(customerId.ToString(), out var parsedCustomerId))
            return await next(context);
        
        var classes = typeof(IHaveCustomerId).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsInterface: false, IsAbstract: false } &&
                typeof(IHaveCustomerId).IsAssignableFrom(type))
            .ToList();
            
        foreach (var @class in classes)
        {
            var parameter = context.Arguments
                .FirstOrDefault(arg => arg?.GetType().IsAssignableTo(@class) ?? false);
            
            if (parameter is null) continue;
            
            var property = @class.GetProperty("CustomerId");
            property?.SetValue(parameter, parsedCustomerId);
        }

        return await next(context);
    }
}