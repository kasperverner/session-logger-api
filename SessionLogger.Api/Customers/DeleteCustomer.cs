using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Customers;
    
public class DeleteCustomer : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapDelete("", Handle)
            .WithSummary("Delete a specific customer")
            .WithRequiredRoles(Role.Manager)
            .WithResponse();
    
    public record Request(Guid CustomerId);
    
    private static async Task<NoContent> Handle(
        [AsParameters] Request request, 
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        await customerService.DeleteCustomerAsync(request.CustomerId, ct);
        
        return TypedResults.NoContent();
    }
}