using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionLogger.Users;

namespace SessionLogger.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(80);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(254);
        
        builder.HasIndex(x => x.PrincipalId).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
        
        builder.HasMany(x => x.OptOuts)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Sessions)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.AssignedTasks)
            .WithMany(x => x.AssignedUsers)
            .UsingEntity<UserTask>();
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}