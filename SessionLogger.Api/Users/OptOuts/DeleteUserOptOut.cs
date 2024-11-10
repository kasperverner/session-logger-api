using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;

namespace SessionLogger.Users.OptOuts;

public class DeleteUserOptOut : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapDelete("", Handle)
            .WithSummary("Delete a specific opt out")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<Request>()
            .WithResponse();
    
    public record Request(Guid UserId, Guid OptOutId);
    
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator(IUserService userService)
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .MustAsync(async (userId, ct) => await userService.UserExistsAsync(userId, ct))
                .WithMessage("User does not exist");
            
            RuleFor(x => x.OptOutId)
                .NotEmpty()
                .MustAsync(async (id, ct) => await userService.OptOutExistsAsync(id, ct))
                .WithMessage("User does not exist");
        }
    }
    
    private static async Task<NoContent> Handle(
        [AsParameters] Request request,
        [FromServices] IUserService userService,
        CancellationToken ct)
    {
        await userService.DeleteOptOutAsync(request.OptOutId, ct);
        
        return TypedResults.NoContent();
    }
}