using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SessionLogger.Contacts;

namespace SessionLogger.Filters.Parameters;

public class ContactIdFromRouteFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var contactId = context.HttpContext.GetRouteValue("contactId");

        if (contactId is null || !Guid.TryParse(contactId.ToString(), out var parsedContactId))
            return await next(context);
        
        var classes = typeof(IHaveContactId).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsInterface: false, IsAbstract: false } &&
                typeof(IHaveContactId).IsAssignableFrom(type))
            .ToList();
            
        foreach (var @class in classes)
        {
            var parameter = context.Arguments
                .FirstOrDefault(arg => arg?.GetType().IsAssignableTo(@class) ?? false);
            
            if (parameter is null) continue;
            
            var property = @class.GetProperty("ContactId");
            property?.SetValue(parameter, parsedContactId);
        }

        return await next(context);
    }
}