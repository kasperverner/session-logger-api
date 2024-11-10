using SessionLogger.Extensions;

namespace SessionLogger.Projects.Tasks.Comments;

public static class CommentsEndpoints
{
    public static IEndpointRouteBuilder MapCommentsEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/comments")
            .MapEndpoint<GetComments>()
            .MapEndpoint<CreateComment>()
            .MapGroup("/{commentId:guid}")
            .MapEndpoint<UpdateComment>()
            .MapEndpoint<DeleteComment>();
        
        return application;
    }
}