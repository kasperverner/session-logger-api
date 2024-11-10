using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;

namespace SessionLogger.Users.OptOuts;
    
public class UpdateUserOptOut : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPut("", Handle)
            .WithSummary("Update a specific opt out")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithRequestValidation<UpdateUserOptOutRequest>()
            .WithResponse();
    
    
    public class RequestValidator : AbstractValidator<UpdateUserOptOutRequest>
    {
        public RequestValidator(IUserService userService)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .MustAsync(async (id, ct) => await userService.OptOutExistsAsync(id, ct))
                .WithMessage("User does not exist");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .MustAsync(async (request, endDate, ct) => !await userService.OptOutStartsBeforeAsync(request.Id, endDate, ct))
                .WithMessage("End date must be after start date");
        }
    }
    
    private static async Task<NoContent> Handle(
        [FromBody] UpdateUserOptOutRequest request,
        [FromServices] IUserService userService,
        CancellationToken ct)
    {
        await userService.UpdateOptOutAsync(request, ct);
        
        return TypedResults.NoContent();
    }
}