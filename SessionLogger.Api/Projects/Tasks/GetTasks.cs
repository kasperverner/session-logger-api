using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Tasks;
using SessionLogger.Users;

namespace SessionLogger.Projects.Tasks;

public class GetTasks : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get all tasks for a specific project")
            .WithName("GetTasks")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<Request>()
            .WithResponse<IEnumerable<TaskResponse>>();

    public record Request(Guid ProjectId);
    
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator(IProjectService projectService)
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .MustAsync(async (projectId, ct) => await projectService.ProjectExistsAsync(projectId, ct))
                .WithMessage("Project does not exist");
        }
    }
    
    private static async Task<Ok<IEnumerable<TaskResponse>>> Handle(
        [AsParameters] Request request,
        [FromServices] ITaskService taskService,
        CancellationToken ct)
    {
        var response = await taskService.GetTasksAsync(request.ProjectId, ct);

        return TypedResults.Ok(response);
    }
}