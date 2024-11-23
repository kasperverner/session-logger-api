using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Core;
using SessionLogger.Exceptions;
using SessionLogger.Infrastructure.Services;
using SessionLogger.Interfaces;
using SessionLogger.Persistence;
using IAuthorizationService = SessionLogger.Interfaces.IAuthorizationService;

namespace SessionLogger;

public static class ConfigureServices
{
    /// <summary>
    /// Configures the application with the necessary services in the dependency injection container.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
    /// <returns>A <see cref="WebApplicationBuilder"/> that can be used to further customize the application.</returns>
    public static WebApplicationBuilder Configure(this WebApplicationBuilder builder)
    {
        builder
            .AddAuthentication()
            .AddAuthorization()
            .AddSwagger()
            .AddLogging()
            .AddServices()
            .AddValidation()
            .AddProblemDetails()
            .AddDbContext();
        
        return builder;
    }

    /// <summary>
    /// Add and configure authentication to the application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
    /// <returns>A <see cref="WebApplicationBuilder"/> that can be used to further customize the application.</returns>
    private static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        var authority = builder.Configuration["Jwt:Authority"];
        var audience = builder.Configuration["Jwt:Audience"];
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            { 
                options.Authority = authority;
                options.Audience = audience;
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false,
                    ValidIssuer = authority,
                    ValidAudience = audience,
                    NameClaimType = "name",
                    
                };
        
                // Log authentication errors to the console for debugging
                if (builder.Environment.IsDevelopment())
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Authentication failed: {context.Exception}");
                            return Task.CompletedTask;
                        }
                    };
            });
        
        return builder;
    }
    
    /// <summary>
    /// Add and configure authorization to the application.
    /// - Require authenticated user
    /// - Require role "admin" to prevent MailMak' clients from accessing the API
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
    /// <returns>A <see cref="WebApplicationBuilder"/> that can be used to further customize the application.</returns>
    private static WebApplicationBuilder AddAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(ClaimTypes.Role, "admin")
                .Build();
        });
        
        return builder;
    }
    
    /// <summary>
    /// Add Swagger documentation to the application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
    /// <returns>A <see cref="WebApplicationBuilder"/> that can be used to further customize the application.</returns>
    private static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title = "Session Logger API";
                document.Info.Description = "An API for managing projects, tasks and sessions.";
                document.Info.Version = "v1";

                return Task.CompletedTask;
            });
        });
        
        return builder;
    }
    
    /// <summary>
    /// Add logging to the application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
    /// <returns>A <see cref="WebApplicationBuilder"/> that can be used to further customize the application.</returns>
    private static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        Logger logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
        
        return builder;
    }
    
    /// <summary>
    /// Add services to the dependency injection container.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
    /// <returns>A <see cref="WebApplicationBuilder"/> that can be used to further customize the application.</returns>
    private static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
        builder.Services.AddScoped<ICustomerService, CustomerService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<ISessionService, SessionService>();
        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.AddScoped<IUserService, UserService>();
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMemoryCache();
        
        return builder;
    }

    /// <summary>
    /// Add validation to the dependency injection container.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
    /// <returns>A <see cref="WebApplicationBuilder"/> that can be used to further customize the application.</returns>
    private static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly, includeInternalTypes: true);
        
        return builder;
    }
    
    /// <summary>
    /// Adds services required for creation of <see cref="ProblemDetails"/> for failed requests and <see cref="ExceptionHandler"/> to the dependency injection container.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
    /// <returns>A <see cref="WebApplicationBuilder"/> that can be used to further customize the application.</returns>
    private static WebApplicationBuilder AddProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });

        builder.Services.AddExceptionHandler<ExceptionHandler>();
        
        return builder;
    }

    /// <summary>
    /// Add and configure the database context to the dependency injection container.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
    /// <returns>A <see cref="WebApplicationBuilder"/> that can be used to further customize the application.</returns>
    private static void AddDbContext(this WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetConnectionString("SessionLogger")!;
        
        builder.Services.AddDbContext<SessionLoggerContext>(options =>
        {
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });
    }
}