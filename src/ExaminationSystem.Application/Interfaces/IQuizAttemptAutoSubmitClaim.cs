namespace ExaminationSystem.Application.Interfaces;

public sealed record OverdueAttemptWorkItem(
    Guid AttemptId,
    Guid UserId,
    bool NeedsInProgressClaim,
    bool RequiresStuckSubmittingReclaim);

public sealed record StuckSubmittingAttemptDiagnostic(
    Guid AttemptId,
    DateTime? ClaimTimestamp,
    double AgeSeconds);

public interface IQuizAttemptAutoSubmitClaim
{
  
    Task<IReadOnlyList<OverdueAttemptWorkItem>> GetOverdueWorkItemsAsync(
        int max,
        DateTime asOfUtc,
        TimeSpan submittingStuckThreshold,
        CancellationToken cancellationToken = default);

    Task<int> TryClaimInProgressOverdueAsync(
        Guid attemptId,
        DateTime asOfUtc,
        CancellationToken cancellationToken = default);

    Task<int> TryReclaimStuckSubmittingAsync(
        Guid attemptId,
        DateTime asOfUtc,
        TimeSpan submittingStuckThreshold,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StuckSubmittingAttemptDiagnostic>> GetStuckSubmittingDiagnosticsAsync(
        int max,
        DateTime asOfUtc,
        TimeSpan submittingStuckThreshold,
        CancellationToken cancellationToken = default);

    Task<double?> GetMaxStuckSubmittingAgeSecondsAsync(
        DateTime asOfUtc,
        TimeSpan submittingStuckThreshold,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Global count of stuck Submitting attempts (no result, claim age past the stuck threshold).
    /// </summary>
    Task<int> GetTotalStuckSubmittingCountAsync(
        DateTime asOfUtc,
        TimeSpan submittingStuckThreshold,
        CancellationToken cancellationToken = default);
}
