using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Customers.Contacts;

// TODO: Modify

public class DeleteContact : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapDelete("", Handle)
            .WithSummary("Delete a specific contact")
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