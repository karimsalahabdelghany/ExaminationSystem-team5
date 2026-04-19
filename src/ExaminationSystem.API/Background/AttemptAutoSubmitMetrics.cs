using System.Diagnostics.Metrics;

namespace ExaminationSystem.API.Background;

/// <summary>
/// OpenTelemetry-compatible meters for auto-submit sweeps (export when OTLP/Prometheus is configured).
/// </summary>
public sealed class AttemptAutoSubmitMetrics
{
    public const string MeterName = "ExaminationSystem.AttemptAutoSubmit";

    private readonly Meter _meter;
    private readonly Counter<long> _reclaimedCount;
    private readonly Counter<long> _processedCount;
    private readonly Counter<long> _claimRowsCount;
    private readonly Counter<long> _failureCount;
    private readonly Histogram<double> _stuckSubmittingAgeMaxSeconds;
    private readonly Histogram<double> _sweepDurationSeconds;

    private long _lastGlobalStuckSubmittingCount;

    public AttemptAutoSubmitMetrics()
    {
        _meter = new Meter(MeterName, "1.0.0");
        _meter.CreateObservableGauge(
            "attempts.submitting.stuck.count",
            () => Volatile.Read(ref _lastGlobalStuckSubmittingCount),
            description: "Global count of stuck Submitting attempts (no result, past SLA).");

        _reclaimedCount = _meter.CreateCounter<long>("attempts.submitting.reclaimed.count");
        _processedCount = _meter.CreateCounter<long>("attempts.autosubmit.processed.count");
        _claimRowsCount = _meter.CreateCounter<long>("attempts.autosubmit.claim.rows");
        _failureCount = _meter.CreateCounter<long>("attempts.autosubmit.failure.count");
        _stuckSubmittingAgeMaxSeconds = _meter.CreateHistogram<double>("attempts.submitting.age.max");
        _sweepDurationSeconds = _meter.CreateHistogram<double>("attempts.autosubmit.sweep.duration");
    }

    public void RecordSweep(
        int globalStuckSubmittingCount,
        double globalMaxStuckAgeSeconds,
        int reclaimedRows,
        int processed,
        int claimRows,
        int failures,
        double sweepDurationSeconds)
    {
        Volatile.Write(ref _lastGlobalStuckSubmittingCount, globalStuckSubmittingCount);

        if (globalStuckSubmittingCount > 0 && globalMaxStuckAgeSeconds > 0)
            _stuckSubmittingAgeMaxSeconds.Record(globalMaxStuckAgeSeconds);

        if (reclaimedRows > 0)
            _reclaimedCount.Add(reclaimedRows);

        if (processed > 0)
            _processedCount.Add(processed);

        if (claimRows > 0)
            _claimRowsCount.Add(claimRows);

        if (failures > 0)
            _failureCount.Add(failures);

        _sweepDurationSeconds.Record(sweepDurationSeconds);
    }
}
