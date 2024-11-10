using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Tasks;
using SessionLogger.Users;

namespace SessionLogger.Projects.Tasks;

public class CreateUserTask : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPost("users", Handle)
            .WithSummary("Assign a user to a specific task")
            .WithRequiredRoles(Role.Manager)
            .WithRequestValidation<CreateUserTaskRequest>()
            .WithResponse();
    
    public class RequestValidator : AbstractValidator<CreateUserTaskRequest>
    {
        public RequestValidator(ITaskService taskService, IUserService userService)
        {
            RuleFor(x => x.TaskId)
                .NotEmpty()
                .MustAsync(async (taskId, ct) => await taskService.TaskExistsAsync(taskId, ct))
                .WithMessage("Project does not exist")
                .MustAsync(async (request, taskId, ct) => !await taskService.TaskExistsAsync(taskId, request.UserId, ct))
                .WithMessage("User is already assigned to this task");
            
            RuleFor(x => x.UserId)
                .NotEmpty()
                .MustAsync(async (userId, ct) => await userService.UserExistsAsync(userId, ct))
                .WithMessage("Project name already exists for this customer")
                .MustAsync(async (userId, ct) => await userService.UserExistsAsync(userId, Role.Employee, ct))
                .WithMessage("User is not an employee");
        }
    }
    
    private static async Task<NoContent> Handle(
        [FromBody] CreateUserTaskRequest request, 
        [FromServices] ITaskService taskService, 
        CancellationToken ct)
    {
        await taskService.AssignUserToTaskAsync(request, ct);

        return TypedResults.NoContent();
    }
}