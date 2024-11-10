using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Exceptions;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Tasks;
using SessionLogger.Users;

namespace SessionLogger.Projects.Tasks;

public class UpdateTask : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPut("", Handle)
            .WithSummary("Update a specific task")
            .WithRequiredRoles(Role.Manager)
            .WithRequestValidation<UpdateTaskRequest>()
            .WithResponse();
    
    public class RequestValidator : AbstractValidator<UpdateTaskRequest>
    {
        public RequestValidator(IProjectService projectService, ITaskService taskService)
        {
            RuleFor(x => x.ProjectId)
                .NotEmpty()
                .MustAsync(async (id, ct) => await projectService.ProjectExistsAsync(id, ct))
                .WithMessage("Project does not exist");
            
            RuleFor(x => x.Id)
                .NotEmpty()
                .MustAsync(async (id, ct) => await taskService.TaskExistsAsync(id, ct))
                .WithMessage("Task does not exist");
            
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(120)
                .MustAsync(async (request, name, ct) => !await taskService.TaskExistsAsync(request.ProjectId, name, request.Id, ct))
                .WithMessage("Project name already exists for this customer");
            
            RuleFor(x => x.Description)
                .MaximumLength(1000);
            
            When(x => x.Type == TaskType.Completable, () =>
            {
                RuleFor(x => x.State)
                    .NotEmpty()
                    .IsInEnum();

                RuleFor(x => x.Deadline)
                    .NotEmpty();
            });
            
            When(x => x.Type == TaskType.Recurring, () =>
            {
                RuleFor(x => x.State)
                    .Empty()
                    .WithMessage("State is not applicable to recurring tasks");
                
                RuleFor(x => x.Deadline)
                    .Empty()
                    .WithMessage("Deadline is not applicable to recurring tasks");
            });
        }
    }
    
    private static async Task<NoContent> Handle(
        [FromRoute] Guid taskId,
        [FromBody] UpdateTaskRequest request,
        [FromServices] ITaskService taskService,
        CancellationToken ct)
    {
        if (taskId != request.Id)
            throw new ProblemException("Task ID mismatch", "The project ID in the request body does not match the task ID in the URL");
        
        await taskService.UpdateTaskAsync(request, ct);
        
        return TypedResults.NoContent();
    }
}