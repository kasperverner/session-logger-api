using SessionLogger.Extensions;
using SessionLogger.Filters.Parameters;
using SessionLogger.Users.Schedules;

namespace SessionLogger.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/users")
            .WithTags("Users")
            .MapEndpoint<GetUsers>()
            .MapGroup("/{userId:guid}")
            .AddEndpointFilter<UserIdFromRouteFilter>()
            .MapEndpoint<GetUser>()
            .MapEndpoint<UpdateUser>()
            .MapEndpoint<DeleteUser>()
            .MapScheduleEndpoints();
        
        return application;
    }
}