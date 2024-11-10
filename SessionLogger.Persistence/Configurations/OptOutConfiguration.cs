using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SessionLogger.Common;
using SessionLogger.Users;

namespace SessionLogger.Persistence.Configurations;

public class OptOutConfiguration : IEntityTypeConfiguration<OptOut>
{
    public void Configure(EntityTypeBuilder<OptOut> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.OwnsOne<Period>(x => x.Period);
        
        builder.HasQueryFilter(x => !x.DeletedDate.HasValue);
    }
}