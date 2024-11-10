using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionLogger.Common;
using SessionLogger.Sessions;

namespace SessionLogger.Persistence.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.UserId);

        builder.HasDiscriminator(x => x.Type)
            .HasValue<CheckInSession>(SessionType.CheckIn)
            .HasValue<ProjectSession>(SessionType.Project);
        
        builder.OwnsOne<Period>(x => x.Period);
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}

public class ProjectSessionConfiguration : IEntityTypeConfiguration<ProjectSession>
{
    public void Configure(EntityTypeBuilder<ProjectSession> builder)
    {
        builder.Property(x => x.Description).HasMaxLength(1000); 
    }
}