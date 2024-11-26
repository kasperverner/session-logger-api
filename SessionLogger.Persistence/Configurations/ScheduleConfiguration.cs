using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionLogger.Common;
using SessionLogger.Schedules;

namespace SessionLogger.Persistence.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasDiscriminator(x => x.Type)
            .HasValue<UserSchedule>(ScheduleType.User)
            .HasValue<ProjectSchedule>(ScheduleType.Project);
        
        builder.OwnsOne<Period>(x => x.Period, period =>
        {
            period.Property(x => x.StartDate).IsRequired();
            period.Property(x => x.EndDate).IsRequired(false);
        });
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}

public class ProjectScheduleConfiguration : IEntityTypeConfiguration<ProjectSchedule>
{
    public void Configure(EntityTypeBuilder<ProjectSchedule> builder)
    {
        builder.HasOne(x => x.Project)
            .WithMany(x => x.Schedule)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.ApprovedBy)
            .WithMany()
            .HasForeignKey(x => x.ApprovedById)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Property(x => x.ApprovedHours).IsRequired();
    }
}

public class UserScheduleConfiguration : IEntityTypeConfiguration<UserSchedule>
{
    public void Configure(EntityTypeBuilder<UserSchedule> builder)
    {
        builder.HasOne(x => x.User)
            .WithMany(x => x.Schedule)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.AbsenceType).IsRequired();
    }
}