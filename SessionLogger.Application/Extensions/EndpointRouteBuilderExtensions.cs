using Microsoft.AspNetCore.Routing;
using SessionLogger.Interfaces;

namespace SessionLogger.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        
        return app;
    }
}