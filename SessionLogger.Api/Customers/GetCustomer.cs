using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SessionLogger.Extensions;
using SessionLogger.Filters;
using SessionLogger.Interfaces;
using SessionLogger.Users;

namespace SessionLogger.Customers;
    
public class GetCustomer : IEndpoint
{
    public static void Map(IEndpointRouteBuilder application)
        => application.MapGet("", Handle)
            .WithSummary("Get a specific customer")
            .WithName("GetCustomer")
            .WithRequiredRoles(Role.Employee | Role.Manager)
            .WithResponse<CustomerResponse>();

    private static async Task<Ok<CustomerResponse>> Handle(
        [FromRoute] Guid customerId,
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        var response = await customerService.GetCustomerAsync(customerId, ct);

        return TypedResults.Ok(response);
    }
}