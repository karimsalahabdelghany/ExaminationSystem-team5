using ExaminationSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ExaminationSystem.Persistence.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;
    //private readonly ICurrentUser _currentUser;
    public AuditableEntityInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
       // _currentUser = _serviceProvider.GetRequiredService<ICurrentUser>();
    }


    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {

        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<IBaseEntity>())
        {
            // Add Logic
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = "system";
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            // Update Logic
            if (entry.Properties.Any(p => p.IsModified))
            {
                entry.Entity.UpdatedBy = "system";
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            // Soft delete logic
            if (entry.State == EntityState.Deleted && entry.Entity is IBaseEntity softDelete)
            {
                entry.State = EntityState.Modified;
                softDelete.IsDeleted = true;
                //softDelete.DeletedBy = "system";
                softDelete.DeletedAt = DateTime.UtcNow;
            }
        }

    }
}
