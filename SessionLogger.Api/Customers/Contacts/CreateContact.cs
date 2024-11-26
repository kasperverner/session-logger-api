using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Customers.Contacts;

// TODO: Modify

public class CreateContact : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPost("", Handle)
            .WithSummary("Create a new contact")
            .WithRequiredRoles(Role.Manager)
            .WithRequestValidation<CreateCustomerRequest>()
            .WithResponse<CustomerResponse>(StatusCodes.Status201Created);
    
    public class RequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public RequestValidator(ICustomerService customerService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(120)
                .MustAsync(async (name, ct) => !await customerService.CustomerExistsAsync(name, ct))
                .WithMessage("Customer name is already in use");
        }
    }
    
    private static async Task<CreatedAtRoute<CustomerResponse>> Handle(
        [FromBody] CreateCustomerRequest request,
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        var response = await customerService.CreateCustomerAsync(request, ct);
        
        return TypedResults.CreatedAtRoute(response, "GetCustomer", new { customerId = response.Id });
    }
}