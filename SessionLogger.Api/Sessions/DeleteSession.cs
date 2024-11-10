using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Sessions;

public class DeleteSession : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapDelete("", Handle)
            .WithSummary("Delete a specific session")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<Request>()
            .WithResponse();
    
    public record Request(Guid SessionId);
    
    private static async Task<NoContent> Handle(
        [AsParameters] Request request,
        [FromServices] ISessionService sessionService,
        CancellationToken ct)
    {
        await sessionService.DeleteSessionAsync(request.SessionId, ct);
        
        return TypedResults.NoContent();
    }
}