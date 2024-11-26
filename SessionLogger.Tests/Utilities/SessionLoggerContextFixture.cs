using Microsoft.EntityFrameworkCore;
using SessionLogger.Persistence;
using SessionLogger.Users;

namespace SessionLogger.Tests.Utilities;

public class SessionLoggerContextFixture : IAsyncLifetime
{
    public SessionLoggerContext Context { get; }

    public SessionLoggerContextFixture()
    {
        var options = new DbContextOptionsBuilder<SessionLoggerContext>()
            .UseInMemoryDatabase("SessionLoggerDatabase")
            .Options;

        Context = new SessionLoggerContext(options);
    }
    
    public SessionLoggerContext GetContext()
    {
        return Context;
    }

    public async Task InitializeAsync()
    {
        await Context.Database.EnsureCreatedAsync();
        await SeedDataAsync();
    }

    private async Task SeedDataAsync()
    {
        var user = new User(TestHelpers.Constants.Users.User.PrincipalId, TestHelpers.Constants.Users.User.Name, TestHelpers.Constants.Users.User.Email);
        user.AssignRoles(Role.Employee | Role.Manager);
        
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.DisposeAsync();
    }
}