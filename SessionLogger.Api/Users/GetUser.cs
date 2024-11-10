using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;

namespace SessionLogger.Users;

public class GetUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get a specific user")
            .WithName("GetUser")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithResponse<UserResponse>();
    
    public record Request(Guid UserId);
    
    private static async Task<Ok<UserResponse>> Handle(
        [AsParameters] Request request,
        [FromServices] IUserService userService,
        CancellationToken ct)
    {
        var user = await userService.GetUserAsync(request.UserId, ct);
        
        return TypedResults.Ok(user);
    }
}