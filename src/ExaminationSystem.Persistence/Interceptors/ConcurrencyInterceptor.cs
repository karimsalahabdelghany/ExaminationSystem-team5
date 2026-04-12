using ExaminationSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ExaminationSystem.Persistence.Interceptors;

public class ConcurrencyInterceptor : ISaveChangesInterceptor
{
    // Intercept and handle concurrency issues before saving changes
    public Task<int> SavingChangesAsync(
        DbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        // Iterate over all entities marked for update
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            if (entry.Entity is IBaseEntity concurrencyEntity)
            {
                var currentRowVersion = entry.OriginalValues["RowVersion"] as byte[];
                var proposedRowVersion = entry.CurrentValues["RowVersion"] as byte[];

                // Check if the RowVersion has been modified by another user
                if (currentRowVersion != null && proposedRowVersion != null && !currentRowVersion.SequenceEqual(proposedRowVersion))
                {
                    throw new DbUpdateConcurrencyException("The entity has been modified by another user.");
                }
            }
        }

        return Task.FromResult(0); // Return an integer Task as required
    }
}