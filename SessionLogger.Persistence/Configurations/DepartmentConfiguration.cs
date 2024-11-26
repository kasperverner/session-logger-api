using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionLogger.Users;

namespace SessionLogger.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name).IsRequired().HasMaxLength(120);
        
        builder.HasIndex(x => new { x.Name }).IsUnique();
        
        builder.HasMany(x => x.Users)
            .WithOne(x => x.Department)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Rates)
            .WithOne(x => x.Department)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}

public class DepartmentRateConfiguration : IEntityTypeConfiguration<DepartmentRate>
{
    public void Configure(EntityTypeBuilder<DepartmentRate> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => new { x.DepartmentId, x.CustomerId }).IsUnique();

        builder.Property(x => x.Rate).HasPrecision(19, 2);
        
        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Rates)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Department)
            .WithMany(x => x.Rates)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}