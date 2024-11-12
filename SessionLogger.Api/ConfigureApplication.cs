using Scalar.AspNetCore;
using SessionLogger.Customers;
using SessionLogger.Filters;
using SessionLogger.Projects;
using SessionLogger.Sessions;
using SessionLogger.Users;

namespace SessionLogger;

public static class ConfigureApplication
{
    /// <summary>
    /// Configures the application with the necessary middleware.
    /// </summary>
    /// <param name="application">The <see cref="WebApplication"/> to add the middleware to.</param>
    /// <returns>A <see cref="WebApplication"/> that can be used to further customize the application.</returns>
    public static WebApplication Configure(this WebApplication application)
    {
        application.MapOpenApi();
        application.MapScalarApiReference(options =>
        {
            options
                .WithTitle("Session Logger API")
                .WithTestRequestButton(false)
                .WithEndpointPrefix("openapi/{documentName}")
                .WithTheme(ScalarTheme.Moon);
        });

        application.UseHttpsRedirection();
        
        application.UseAuthentication();
        application.UseAuthorization();

        application.UseExceptionHandler();
        
        application.MapEndpoints();
        
        return application;
    }
    
    /// <summary>
    /// Configures the endpoints for the application.
    /// </summary>
    /// <param name="application">The <see cref="WebApplication"/> to add the middleware to.</param>
    /// <returns>A <see cref="WebApplication"/> that can be used to further customize the application.</returns>
    private static WebApplication MapEndpoints(this WebApplication application)
    {
        application.MapGroup("")
            .WithOpenApi()
            .AddEndpointFilter<RequestLoggingFilter>()
            .RequireAuthorization()
            .MapCustomersEndpoints()
            .MapProjectsEndpoints()
            .MapSessionsEndpoints()
            .MapUsersEndpoints();
        
        return application;
    }
}