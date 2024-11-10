using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Sessions;

public class GetSessions : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get all sessions")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithResponse<IEnumerable<SessionResponse>>();
    
    private static async Task<Ok<IEnumerable<SessionResponse>>> Handle(
        [FromServices] ISessionService sessionService,
        CancellationToken ct)
    {
        var sessions = await sessionService.GetSessionsAsync(ct);
        
        return TypedResults.Ok(sessions);
    }
}