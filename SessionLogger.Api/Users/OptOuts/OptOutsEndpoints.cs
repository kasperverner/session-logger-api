using SessionLogger.Extensions;

namespace SessionLogger.Users.OptOuts;

public static class OptOutsEndpoints
{
    public static IEndpointRouteBuilder MapUOptOutsEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/opt-outs")
            .MapEndpoint<GetUserOptOuts>()
            .MapEndpoint<CreateUserOptOut>()
            .MapGroup("/{optOutId:guid}")
            .MapEndpoint<UpdateUserOptOut>()
            .MapEndpoint<DeleteUserOptOut>();
        
        return application;
    }
}