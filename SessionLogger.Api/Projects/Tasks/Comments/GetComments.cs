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

// TODO: Modify

public class GetComments : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get all comments for a task")
            .WithName("GetComments")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<Request>()
            .WithResponse<IEnumerable<CommentResponse>>();

    public record Request(Guid TaskId);
    
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator(ITaskService taskService)
        {
            RuleFor(x => x.TaskId)
                .NotEmpty()
                .MustAsync(async (taskId, ct) => await taskService.TaskExistsAsync(taskId, ct))
                .WithMessage("Task does not exist");
        }
    }
    
    private static async Task<Ok<IEnumerable<CommentResponse>>> Handle(
        [AsParameters] Request request,
        [FromServices] ITaskService taskService,
        CancellationToken ct)
    {
        var response = await taskService.GetCommentsAsync(request.TaskId, ct);

        return TypedResults.Ok(response);
    }
}