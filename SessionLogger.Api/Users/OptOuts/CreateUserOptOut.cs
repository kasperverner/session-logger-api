using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;

namespace SessionLogger.Users.OptOuts;
    
public class CreateUserOptOut  : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPost("", Handle)
            .WithSummary("Create a new opt out")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<CreateUserOptOutRequest>()
            .WithResponse<OptOutResponse>(StatusCodes.Status201Created);
    
    public class RequestValidator : AbstractValidator<CreateUserOptOutRequest>
    {
        public RequestValidator(IUserService userService)
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .MustAsync(async (userId, ct) => await userService.UserExistsAsync(userId, ct))
                .WithMessage("User does not exist")
                .MustAsync(async (userId, ct) => !await userService.UserHasActiveOptOUtAsync(userId, ct))
                .WithMessage("User has an active opt out");

            RuleFor(x => x.StartDate)
                .NotEmpty();
            
            RuleFor(x => x.EndDate)
                .Must((request, endDate) => !endDate.HasValue || endDate > request.StartDate)
                .WithMessage("End date must be after start date");
        }
    }
    
    private static async Task<CreatedAtRoute<OptOutResponse>> Handle(
        [FromBody] CreateUserOptOutRequest request,
        [FromServices] IUserService userService,
        CancellationToken ct)
    {
        var optOut = await userService.CreateOptOutAsync(request, ct);
        
        return TypedResults.CreatedAtRoute(optOut, "GetUserOptOuts", new { UserId = request.UserId });
    }
}