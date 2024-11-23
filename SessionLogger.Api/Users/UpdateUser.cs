using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SessionLogger.Exceptions;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Persistence;

namespace SessionLogger.Users;
    
public class UpdateUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPut("", Handle)
            .WithSummary("Update a specific user")
            .WithRequiredRoles(Role.Manager)
            .WithRequestValidation<UpdateUserRequest>()
            .WithResponse();
    
    public class RequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public RequestValidator(IUserService userService)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .MustAsync(async (userId, ct) => await userService.UserExistsAsync(userId, ct))
                .WithMessage("User does not exist");

            RuleFor(x => x.Roles)
                .NotEmpty()
                .Must(x => x.HasFlag(Role.None | Role.Employee | Role.Manager))
                .WithMessage("Invalid role");
        }
    }
    
    private static async Task<NoContent> Handle(
        [FromRoute] Guid userId, 
        [FromBody] UpdateUserRequest request, 
        [FromServices] IUserService userService,
        CancellationToken ct)
    {
        if (userId != request.Id)
            throw new ProblemException("User ID mismatch", "The user ID in the request body does not match the user ID in the URL");
        
        await userService.UpdateUserAsync(request, ct);
        
        return TypedResults.NoContent();
    }
}