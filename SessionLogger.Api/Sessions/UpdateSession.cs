using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Sessions;

public class UpdateSession : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPut("", Handle)
            .WithSummary("Update a specific session")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<UpdateSessionRequest>()
            .WithResponse();
    
    
    public class RequestValidator : AbstractValidator<UpdateSessionRequest>
    {
        public RequestValidator(ITaskService taskService, ISessionService sessionService)
        {
            RuleFor(x => x.TaskId)
                .NotEmpty()
                .MustAsync(async (id, ct) => await taskService.TaskExistsAsync(id, ct))
                .WithMessage("Task does not exist");
            
            RuleFor(x => x.SessionId)
                .NotEmpty()
                .MustAsync(async (id, ct) => await sessionService.SessionExistsAsync(id, ct))
                .MustAsync(async (id, ct) => !await sessionService.SessionExistsAsync(id, SessionState.Billed, ct))
                .WithMessage("Session does not exist");

            RuleFor(x => x.Description)
                .MaximumLength(1000);
            
            RuleFor(x => x.EndDate)
                .Must((request, endDate) => !endDate.HasValue || !request.StartDate.HasValue || endDate > request.StartDate) 
                .WithMessage("End date must be after start date");
        }
    }
    
    private static async Task<NoContent> Handle(
        [FromBody] UpdateSessionRequest request,
        [FromServices] ISessionService sessionService,
        CancellationToken ct)
    {
        await sessionService.UpdateSessionAsync(request, ct);
        
        return TypedResults.NoContent();
    }
}