using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SessionLogger.Exceptions;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Persistence;
using SessionLogger.Users;

namespace SessionLogger.Customers;

// TODO: Modify

public class UpdateCustomer : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapPut("", Handle)
            .WithSummary("Update a specific customer")
            .WithRequiredRoles(Role.Manager)
            .WithRequestValidation<UpdateCustomerRequest>()
            .WithResponse();
    
    public class RequestValidator : AbstractValidator<UpdateCustomerRequest>
    {
        public RequestValidator(ICustomerService customerService)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .MustAsync(async (id, ct) => await customerService.CustomerExistsAsync(id, ct))
                .WithMessage("Customer does not exist");
            
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(120)
                .MustAsync((async (request, name, token) => !await customerService.CustomerExistsAsync(name, request.Id, token)))
                .WithMessage("Customer name is already in use");
        }
    }
    
    private static async Task<NoContent> Handle(
        [FromRoute] Guid customerId,
        [FromBody] UpdateCustomerRequest request,
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        if (customerId != request.Id)
            throw new ProblemException("Customer ID mismatch", "The customer ID in the request body does not match the customer ID in the URL");
        
        await customerService.UpdateCustomerAsync(request, ct);
        
        return TypedResults.NoContent();
    }
}