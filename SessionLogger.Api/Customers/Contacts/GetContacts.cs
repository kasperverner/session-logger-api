using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Customers.Contacts;

// TODO: Modify

public class GetContacts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get all customers")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithResponse<IEnumerable<CustomerResponse>>();
    
    private static async Task<Ok<IEnumerable<CustomerResponse>>> Handle(
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        var response = await customerService.GetCustomersAsync(ct);

        return TypedResults.Ok(response);
    }
}