using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Tasks;
using SessionLogger.Users;

namespace SessionLogger.Projects.Tasks;

// TODO: Modify

public class GetTask : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get a specific task")
            .WithName("GetTask")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithResponse<TaskResponse>();
    
    public record Request(Guid TaskId);
    
    private static async Task<Ok<TaskResponse>> Handle(
        [AsParameters] Request request,
        [FromServices] ITaskService taskService,
        CancellationToken ct)
    {
        var task = await taskService.GetTaskAsync(request.TaskId, ct);
        
        return TypedResults.Ok(task);
    }
}