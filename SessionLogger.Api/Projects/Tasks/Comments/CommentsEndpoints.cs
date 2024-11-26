using SessionLogger.Extensions;
using SessionLogger.Filters.Parameters;

namespace SessionLogger.Projects.Tasks.Comments;

public static class CommentsEndpoints
{
    public static IEndpointRouteBuilder MapCommentsEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/comments")
            .WithTags("Comments")
            .MapEndpoint<GetComments>()
            .MapEndpoint<CreateComment>()
            .MapGroup("/{commentId:guid}")
            .AddEndpointFilter<CommentIdFromRouteFilter>()
            .MapEndpoint<UpdateComment>()
            .MapEndpoint<DeleteComment>();
        
        return application;
    }
}