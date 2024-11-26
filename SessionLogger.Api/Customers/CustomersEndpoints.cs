using SessionLogger.Customers.Contacts;
using SessionLogger.Extensions;
using SessionLogger.Filters.Parameters;

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
            .AddEndpointFilter<CustomerIdFromRouteFilter>()
            .MapEndpoint<GetCustomer>()
            .MapEndpoint<UpdateCustomer>()
            .MapEndpoint<DeleteCustomer>()
            .MapContactsEndpoints();
        
        return application;
    }
}