using SessionLogger.Extensions;
using SessionLogger.Users.OptOuts;

namespace SessionLogger.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application
            .MapGroup("/users")
            .WithTags("Users")
            .MapEndpoint<GetUsers>()
            .MapGroup("/{userId:guid}")
            .MapEndpoint<GetUser>()
            .MapEndpoint<UpdateUser>()
            .MapEndpoint<DeleteUser>()
            .MapUOptOutsEndpoints();
        
        return application;
    }
}