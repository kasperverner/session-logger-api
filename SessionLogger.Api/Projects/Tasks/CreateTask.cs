using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Tasks;
using SessionLogger.Users;

namespace SessionLogger.Projects.Tasks;

public class CreateTask : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPost("", Handle)
            .WithSummary("Create a new task")
            .WithRequiredRoles(Role.Manager)
            .WithRequestValidation<CreateTaskRequest>()
            .WithResponse();
    
    public class RequestValidator : AbstractValidator<CreateTaskRequest>
    {
        public RequestValidator(IProjectService projectService, ITaskService taskService)
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .MustAsync(async (id, ct) => await projectService.ProjectExistsAsync(id, ct))
                .WithMessage("Project does not exist");
            
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(120)
                .MustAsync(async (request, name, ct) => !await taskService.TaskExistsAsync(request.ProjectId, name, ct))
                .WithMessage("Project name already exists for this customer");
            
            RuleFor(x => x.Description)
                .MaximumLength(1000);
            
            When(x => x.Type == TaskType.Completable, () =>
            {
                RuleFor(x => x.Deadline)
                    .NotEmpty();
            });
            
            When(x => x.Type == TaskType.Recurring, () =>
            {
                RuleFor(x => x.Deadline)
                    .Empty()
                    .WithMessage("Deadline is not applicable to recurring tasks");
            });
        }
    }
    
    private static async Task<CreatedAtRoute<TaskResponse>> Handle(
        [FromBody] CreateTaskRequest request, 
        [FromServices] ITaskService taskService, 
        CancellationToken ct)
    {
        var response = await taskService.CreateTaskAsync(request, ct);

        return TypedResults.CreatedAtRoute(response, "GetTasks", new { projectId = request.ProjectId });
    }
}