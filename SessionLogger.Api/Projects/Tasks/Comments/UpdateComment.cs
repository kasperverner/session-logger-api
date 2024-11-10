using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Comments;
using SessionLogger.Exceptions;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Tasks;
using SessionLogger.Users;

namespace SessionLogger.Projects.Tasks.Comments;

public class UpdateComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPut("", Handle)
            .WithSummary("Update a specific comment for a task")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<UpdateCommentRequest>()
            .WithResponse();
    
    public class RequestValidator : AbstractValidator<UpdateCommentRequest>
    {
        public RequestValidator(ITaskService taskService)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .MustAsync(async (id, ct) => await taskService.CommentExistsAsync(id, ct))
                .WithMessage("Comment does not exist");

            RuleFor(x => x.Value)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }
    
    private static async Task<NoContent> Handle(
        [FromRoute] Guid commentId,
        [FromBody] UpdateCommentRequest request,
        [FromServices] ITaskService taskService,
        CancellationToken ct)
    {
        if (commentId != request.Id)
            throw new ProblemException("Comment ID mismatch", "The project ID in the request body does not match the comment ID in the URL");
        
        await taskService.UpdateCommentAsync(request, ct);
        
        return TypedResults.NoContent();
    }
}