using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;

namespace SessionLogger.Users;

public class GetUsers : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get all users")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithResponse<IEnumerable<UserResponse>>();
    
    private static async Task<Ok<IEnumerable<UserResponse>>> Handle(
        [FromServices] IUserService userService,
        CancellationToken ct)
    {
        var users = await userService.GetUsersAsync(ct);
        
        return TypedResults.Ok(users);
    }
}