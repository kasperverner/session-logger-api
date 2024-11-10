using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SessionLogger.Filters;

namespace SessionLogger.Extensions;

public static class RouteHandlerBuilderValidationExtensions
{
    /// <summary>
    /// Adds the default problem responses to the route handler.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    private static RouteHandlerBuilder ProducesDefaultProblems(this RouteHandlerBuilder builder)
    {
        builder
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return builder;
    }

    /// <summary>
    /// Adds the default problem responses to the route handler.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="statusCodes"><see cref="StatusCodes"/> for a success response</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder WithResponse(this RouteHandlerBuilder builder, int statusCodes = StatusCodes.Status204NoContent)
    {
        builder
            .Produces(statusCodes)
            .ProducesDefaultProblems();

        return builder;
    }
    
    /// <summary>
    /// Adds the default problem responses to the route handler.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="statusCodes"><see cref="StatusCodes"/> for a success response</param>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder WithResponse<TResponse>(this RouteHandlerBuilder builder, int statusCodes = StatusCodes.Status200OK) where TResponse : class
    {
        builder
            .Produces<TResponse>(statusCodes)
            .ProducesDefaultProblems();

        return builder;
    }
    
    /// <summary>
    /// Adds a request validation filter to the route handler.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="builder"></param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }
}