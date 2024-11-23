using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SessionLogger.Infrastructure.Services;
using SessionLogger.Persistence;

namespace SessionLogger.IntegrationTests.Utilities;

public class SessionLoggerContextFixture : IDisposable
{
    public SessionLoggerContext Context { get; }

    public SessionLoggerContextFixture()
    {
        var options = new DbContextOptionsBuilder<SessionLoggerContext>()
            .UseInMemoryDatabase("SessionLoggerDatabase")
            .Options;

        Context = new SessionLoggerContext(options);;
        
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }

    public void Dispose() 
        => Context.Dispose();
}