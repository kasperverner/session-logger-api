using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionLogger.Customers;

namespace SessionLogger.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name).IsRequired().HasMaxLength(120);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(x => x.Projects)
            .WithOne()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Contacts)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Rates)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}

public class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
{
    public void Configure(EntityTypeBuilder<CustomerContact> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(254);
        builder.Property(x => x.Title).HasMaxLength(120);
        
        builder.HasIndex(x => x.Email).IsUnique();
        
        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Contacts)
            .HasForeignKey(x => x.CustomerId);
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}