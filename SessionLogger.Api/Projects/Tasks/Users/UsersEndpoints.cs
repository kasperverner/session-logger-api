using SessionLogger.Extensions;

namespace SessionLogger.Projects.Tasks.Users;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/users")
            .WithTags("Assigned users")
            .MapEndpoint<GetAssignedUsers>()
            .MapEndpoint<CreateAssignedUsers>()
            .MapEndpoint<DeleteAssignedUsers>();
        
        return application;
    }
}