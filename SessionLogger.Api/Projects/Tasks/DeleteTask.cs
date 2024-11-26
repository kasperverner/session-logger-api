using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Projects.Tasks;

// TODO: Modify

public class DeleteTask : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapDelete("", Handle)
            .WithSummary("Delete a specific task")
            .WithRequiredRoles(Role.Manager)
            .WithResponse();
    
    public record Request(Guid TaskId);
    
    private static async Task<NoContent> Handle(
        [AsParameters] Request request,
        [FromServices] ITaskService taskService,
        CancellationToken ct)
    {
        await taskService.DeleteTaskAsync(request.TaskId, ct);
        
        return TypedResults.NoContent();
    }
}