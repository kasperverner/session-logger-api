using Microsoft.EntityFrameworkCore;
using SessionLogger.Customers;
using SessionLogger.Projects;
using SessionLogger.Sessions;
using SessionLogger.Tasks;
using SessionLogger.Users;
using Task = SessionLogger.Tasks.Task;

namespace SessionLogger.Persistence;

public class SessionLoggerContext : DbContext
{
    public SessionLoggerContext(DbContextOptions<SessionLoggerContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .ApplyConfigurationsFromAssembly(typeof(SessionLoggerContext).Assembly);
    
    public DbSet<User> Users => Set<User>();
    public DbSet<OptOut> OptOuts => Set<OptOut>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Task> Tasks => Set<Task>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Session> Sessions => Set<Session>();
}