using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SessionLogger.Comments;

namespace SessionLogger.Filters.Parameters;

public class CommentIdFromRouteFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var commentId = context.HttpContext.GetRouteValue("commentId");

        if (commentId is null || !Guid.TryParse(commentId.ToString(), out var parsedCommentId))
            return await next(context);
        
        var classes = typeof(IHaveCommentId).Assembly
            .GetTypes()
            .Where(type =>
                type is { IsInterface: false, IsAbstract: false } &&
                typeof(IHaveCommentId).IsAssignableFrom(type))
            .ToList();
            
        foreach (var @class in classes)
        {
            var parameter = context.Arguments
                .FirstOrDefault(arg => arg?.GetType().IsAssignableTo(@class) ?? false);
            
            if (parameter is null) continue;
            
            var property = @class.GetProperty("CommentId");
            property?.SetValue(parameter, parsedCommentId);
        }

        return await next(context);
    }
}