using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Persistence;

namespace SessionLogger.Users.OptOuts;
    
public class GetUserOptOuts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get users opt outs")
            .WithName("GetUserOptOuts")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<Request>()      
            .WithResponse<IEnumerable<OptOutResponse>>();
    
    public record Request(Guid UserId);
    
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator(SessionLoggerContext context)
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .MustAsync(async (userId, ct) => await context.Users.AnyAsync(x => x.Id == userId && !x.DeletedDate.HasValue, ct))
                .WithMessage("User does not exist");
        }
    }
    
    private static async Task<Ok<IEnumerable<OptOutResponse>>> Handle(
        [FromBody] Request request,
        [FromServices] IUserService userService,
        CancellationToken ct)
    {
        var optOuts = await userService.GetUserOptOutsAsync(request.UserId, ct);
        
        return TypedResults.Ok(optOuts);
    }
}