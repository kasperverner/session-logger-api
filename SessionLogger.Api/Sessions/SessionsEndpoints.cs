using SessionLogger.Extensions;

namespace SessionLogger.Sessions;

public static class SessionsEndpoints
{
    public static IEndpointRouteBuilder MapSessionsEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application.MapGroup("/sessions")
            .WithTags("Sessions")
            .MapEndpoint<GetSessions>()
            .MapEndpoint<CreateSession>()
            .MapGroup("/{sessionId:guid}")
            .MapEndpoint<GetSession>()
            .MapEndpoint<UpdateSession>()
            .MapEndpoint<DeleteSession>();
        
        return application;
    }
}