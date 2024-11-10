using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;

namespace SessionLogger.Users;
    
public class DeleteUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapDelete("", Handle)
            .WithSummary("Delete a specific user")
            .WithRequiredRoles(Role.Manager)
            .WithResponse();
    
    public record Request(Guid UserId);
    
    private static async Task<NoContent> Handle(
        [AsParameters] Request request,
        [FromServices] IUserService userService,
        CancellationToken ct)
    {
        await userService.DeleteUserAsync(request.UserId, ct);
        
        return TypedResults.NoContent();
    }
}