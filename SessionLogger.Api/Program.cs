using Serilog;

namespace SessionLogger;

public class Program
{
    public static async Task Main(string[] args)
    {
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
        catch(HostAbortedException ex)
        {
            Log.Information("Stopping SessionLogger.Api");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "SessionLogger.Api terminated unexpectedly");
        } 
        finally
        {
            Log.Information("SessionLogger.Api stopped");
            await Log.CloseAndFlushAsync();
        }
    }
}