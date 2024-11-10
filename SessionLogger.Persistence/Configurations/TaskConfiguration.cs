using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionLogger.Tasks;
using Task = SessionLogger.Tasks.Task;

namespace SessionLogger.Persistence.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Description).IsRequired(false).HasMaxLength(1000);
        
        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => new { x.ProjectId, x.Name }).IsUnique();

        builder.HasDiscriminator(x => x.Type)
            .HasValue<CompletableTask>(TaskType.Completable)
            .HasValue<RecurringTask>(TaskType.Recurring);

        builder.HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}