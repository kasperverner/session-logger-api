using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SessionLogger.Persistence.Interceptors;

public class EntityInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var modifiedRecords = eventData.Context?.ChangeTracker.Entries<Entity>() ?? [];
        
        foreach (var entry in modifiedRecords)
            if (entry.State is EntityState.Modified)
                entry.Entity.Modify();

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}