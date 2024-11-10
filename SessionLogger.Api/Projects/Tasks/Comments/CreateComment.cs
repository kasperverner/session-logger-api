using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Comments;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Tasks;
using SessionLogger.Users;

namespace SessionLogger.Projects.Tasks.Comments;

public class CreateComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPost("", Handle)
            .WithSummary("Create a new task")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<CreateCommentRequest>()
            .WithResponse();
    
    public class RequestValidator : AbstractValidator<CreateCommentRequest>
    {
        public RequestValidator(ITaskService taskService)
        {
            RuleFor(x => x.TaskId)
                .NotEmpty()
                .MustAsync(async (id, ct) => await taskService.TaskExistsAsync(id, ct))
                .WithMessage("Task does not exist");

            RuleFor(x => x.Value)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }
    
    private static async Task<CreatedAtRoute<CommentResponse>> Handle(
        [FromRoute] Guid projectId,
        [FromBody] CreateCommentRequest request, 
        [FromServices] ITaskService taskService, 
        CancellationToken ct)
    {
        var response = await taskService.CreateCommentAsync(request, ct);

        return TypedResults.CreatedAtRoute(response, "GetComments", new { projectId, taskId = request.TaskId });
    }
}