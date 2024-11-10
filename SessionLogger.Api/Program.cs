using Serilog;
using SessionLogger;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting SessionLogger.Api");

    // Configure the application
    WebApplicationBuilder builder = WebApplication
        .CreateBuilder(args)
        .Configure();

    // Create the application
    WebApplication application = builder
        .Build()
        .Configure();
    
    // Run the application
    await application.RunAsync();
} 
catch (Exception ex)
{
    Log.Fatal(ex, "SessionLogger.Api terminated unexpectedly");
} 
finally
{
    Log.Information("Stopping SessionLogger.Api");
    Log.CloseAndFlush();
}