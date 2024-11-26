using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Projects.Tasks.Comments;

// TODO: Modify

public class DeleteComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapDelete("", Handle)
            .WithSummary("Delete a specific comment for a task")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithResponse();
    
    public record Request(Guid TaskId);
    
    private static async Task<NoContent> Handle(
        [FromRoute] Guid commentId,
        [FromServices] ITaskService taskService,
        CancellationToken ct)
    {
        await taskService.DeleteCommentAsync(commentId, ct);
        
        return TypedResults.NoContent();
    }
}