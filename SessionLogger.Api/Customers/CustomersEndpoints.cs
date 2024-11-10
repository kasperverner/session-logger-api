using SessionLogger.Extensions;

namespace SessionLogger.Customers;

public static class CustomersEndpoints
{
    public static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/customers")
            .WithTags("Customers")
            .MapEndpoint<GetCustomers>()
            .MapEndpoint<CreateCustomer>()
            .MapGroup("/{customerId:guid}")
            .MapEndpoint<GetCustomer>()
            .MapEndpoint<UpdateCustomer>()
            .MapEndpoint<DeleteCustomer>();
        
        return application;
    }
}