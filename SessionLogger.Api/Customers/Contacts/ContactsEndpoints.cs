using SessionLogger.Extensions;
using SessionLogger.Filters.Parameters;

namespace SessionLogger.Customers.Contacts;

public static class ContactsEndpoints
{
    public static IEndpointRouteBuilder MapContactsEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/contacts")
            .WithTags("Customer Contacts")
            .MapEndpoint<GetContacts>()
            .MapEndpoint<Customers.CreateCustomer>()
            .MapGroup("/{contactId:guid}")
            .AddEndpointFilter<ContactIdFromRouteFilter>()
            .MapEndpoint<GetContact>()
            .MapEndpoint<UpdateContact>()
            .MapEndpoint<DeleteContact>();
        
        return application;
    }
}