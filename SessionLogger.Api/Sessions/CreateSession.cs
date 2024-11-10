using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Projects;
using SessionLogger.Users;

namespace SessionLogger.Sessions;

// - Create a new session
// POST /sessions
// - returns the created session dto

public class CreateSession : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPost("", Handle)
            .WithSummary("Create a new project session")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<CreateSessionRequest>()
            .WithResponse<SessionResponse>();

    public class RequestValidator : AbstractValidator<CreateSessionRequest>
    {
        public RequestValidator(ITaskService taskService)
        {
            RuleFor(x => x.TaskId)
                .NotEmpty()
                .MustAsync(async (id, ct) => await taskService.TaskExistsAsync(id, ct))
                .WithMessage("Task does not exist");

            RuleFor(x => x.Description)
                .MaximumLength(1000);
            
            RuleFor(x => x.EndDate)
                .Must((request, endDate) => !endDate.HasValue || !request.StartDate.HasValue || endDate > request.StartDate) 
                .WithMessage("End date must be after start date");
        }
    }
    
    private static async Task<CreatedAtRoute<SessionResponse>> Handle(
        [FromBody] CreateSessionRequest request,
        [FromServices] ISessionService sessionService,
        CancellationToken ct)
    {
        var response = await sessionService.CreateProjectSessionAsync(request, ct);

        return TypedResults.CreatedAtRoute(response, "GetSession", new { sessionId = response.Id });
    }
}