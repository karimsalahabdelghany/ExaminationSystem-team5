using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Persistence.Services;

public sealed class QuizAttemptAutoSubmitClaim(ApplicationContext context) : IQuizAttemptAutoSubmitClaim
{
    public async Task<IReadOnlyList<OverdueAttemptWorkItem>> GetOverdueWorkItemsAsync(
        int max,
        DateTime asOfUtc,
        TimeSpan submittingStuckThreshold,
        CancellationToken cancellationToken = default)
    {
        var take = Math.Clamp(max, 1, 500);
        var stuckCutoff = asOfUtc - submittingStuckThreshold;

        return await context.QuizAttempts
            .AsNoTracking()
            .Where(a => !a.IsDeleted
                && a.Deadline < asOfUtc
                && (
                    a.Status == QuizAttemptStatus.InProgress
                    || (a.Status == QuizAttemptStatus.Submitting
                        && a.Result == null
                        && (
                            (a.SubmittedAt != null && a.SubmittedAt < stuckCutoff)
                            || (a.SubmittedAt == null && a.UpdatedAt != null && a.UpdatedAt < stuckCutoff)
                            || (a.SubmittedAt == null && a.UpdatedAt == null && a.CreatedAt < stuckCutoff)
                        ))
                    )
                )
            .OrderBy(a => a.Deadline)
            .Take(take)
            .Select(a => new OverdueAttemptWorkItem(
                a.Id,
                a.UserId,
                a.Status == QuizAttemptStatus.InProgress,
                a.Status == QuizAttemptStatus.Submitting))
            .ToListAsync(cancellationToken);
    }

    public Task<int> TryClaimInProgressOverdueAsync(
        Guid attemptId,
        DateTime asOfUtc,
        CancellationToken cancellationToken = default) =>
        context.QuizAttempts
            .Where(a => a.Id == attemptId
                && !a.IsDeleted
                && a.Status == QuizAttemptStatus.InProgress
                && a.Deadline < asOfUtc)
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(a => a.Status, QuizAttemptStatus.Submitting)
                    .SetProperty(a => a.SubmittedAt, asOfUtc)
                    .SetProperty(a => a.UpdatedAt, asOfUtc),
                cancellationToken);

    public Task<int> TryReclaimStuckSubmittingAsync(
        Guid attemptId,
        DateTime asOfUtc,
        TimeSpan submittingStuckThreshold,
        CancellationToken cancellationToken = default)
    {
        var stuckCutoff = asOfUtc - submittingStuckThreshold;
        return context.QuizAttempts
            .Where(a => a.Id == attemptId
                && !a.IsDeleted
                && a.Status == QuizAttemptStatus.Submitting
                && a.Result == null
                && ((a.SubmittedAt != null && a.SubmittedAt < stuckCutoff)
                    || (a.SubmittedAt == null && a.UpdatedAt != null && a.UpdatedAt < stuckCutoff)
                    || (a.SubmittedAt == null && a.UpdatedAt == null && a.CreatedAt < stuckCutoff)))
            .ExecuteUpdateAsync(
                s => s
                    .SetProperty(a => a.SubmittedAt, asOfUtc)
                    .SetProperty(a => a.UpdatedAt, asOfUtc),
                cancellationToken);
    }

    public async Task<IReadOnlyList<StuckSubmittingAttemptDiagnostic>> GetStuckSubmittingDiagnosticsAsync(
        int max,
        DateTime asOfUtc,
        TimeSpan submittingStuckThreshold,
        CancellationToken cancellationToken = default)
    {
        var take = Math.Clamp(max, 1, 500);
        var stuckCutoff = asOfUtc - submittingStuckThreshold;

        var rows = await context.QuizAttempts
            .AsNoTracking()
            .Where(a => !a.IsDeleted
                && a.Status == QuizAttemptStatus.Submitting
                && a.Result == null
                && ((a.SubmittedAt != null && a.SubmittedAt < stuckCutoff)
                    || (a.SubmittedAt == null && a.UpdatedAt != null && a.UpdatedAt < stuckCutoff)
                    || (a.SubmittedAt == null && a.UpdatedAt == null && a.CreatedAt < stuckCutoff)))
            .OrderBy(a => a.SubmittedAt ?? a.UpdatedAt ?? a.CreatedAt)
            .Take(take)
            .Select(a => new
            {
                a.Id,
                ClaimTs = a.SubmittedAt ?? a.UpdatedAt ?? a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return rows
            .Select(r => new StuckSubmittingAttemptDiagnostic(
                r.Id,
                r.ClaimTs,
                (asOfUtc - r.ClaimTs).TotalSeconds))
            .ToList();
    }

    public async Task<double?> GetMaxStuckSubmittingAgeSecondsAsync(
        DateTime asOfUtc,
        TimeSpan submittingStuckThreshold,
        CancellationToken cancellationToken = default)
    {
        var stuckCutoff = asOfUtc - submittingStuckThreshold;
        var ages = await context.QuizAttempts
            .AsNoTracking()
            .Where(a => !a.IsDeleted
                && a.Status == QuizAttemptStatus.Submitting
                && a.Result == null
                && ((a.SubmittedAt != null && a.SubmittedAt < stuckCutoff)
                    || (a.SubmittedAt == null && a.UpdatedAt != null && a.UpdatedAt < stuckCutoff)
                    || (a.SubmittedAt == null && a.UpdatedAt == null && a.CreatedAt < stuckCutoff)))
            .Select(a => (asOfUtc - (a.SubmittedAt ?? a.UpdatedAt ?? a.CreatedAt)).TotalSeconds)
            .ToListAsync(cancellationToken);

        return ages.Count == 0 ? null : ages.Max();
    }

    public Task<int> GetTotalStuckSubmittingCountAsync(
        DateTime asOfUtc,
        TimeSpan submittingStuckThreshold,
        CancellationToken cancellationToken = default)
    {
        var stuckCutoff = asOfUtc - submittingStuckThreshold;
        return context.QuizAttempts
            .AsNoTracking()
            .Where(a => !a.IsDeleted
                && a.Status == QuizAttemptStatus.Submitting
                && a.Result == null
                && ((a.SubmittedAt != null && a.SubmittedAt < stuckCutoff)
                    || (a.SubmittedAt == null && a.UpdatedAt != null && a.UpdatedAt < stuckCutoff)
                    || (a.SubmittedAt == null && a.UpdatedAt == null && a.CreatedAt < stuckCutoff)))
            .CountAsync(cancellationToken);
    }
}
