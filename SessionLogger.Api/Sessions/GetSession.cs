using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Sessions;

public class GetSession : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get all sessions")
            .WithName("GetSession")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithResponse<SessionResponse>();
    
    public record Request(Guid SessionId);
    
    private static async Task<Ok<SessionResponse>> Handle(
        [AsParameters] Request request,
        [FromServices] ISessionService sessionService,
        CancellationToken ct)
    {
        var session = await sessionService.GetSessionAsync(request.SessionId, ct);
        
        return TypedResults.Ok(session);
    }
}