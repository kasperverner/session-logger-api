using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using SessionLogger.Interfaces;
using SessionLogger.Persistence;

namespace SessionLogger.Tests.Utilities;

public class SessionLoggerApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SessionLoggerContextFixture _fixture;

    public SessionLoggerApplicationFactory(SessionLoggerContextFixture fixture)
    {
        _fixture = fixture;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContext));
            
            if (descriptor != null)
                services.Remove(descriptor);
            
            services.AddDbContext<SessionLoggerContext>(_ => _fixture.GetContext());
        });

        return base.CreateHost(builder);
    }
}