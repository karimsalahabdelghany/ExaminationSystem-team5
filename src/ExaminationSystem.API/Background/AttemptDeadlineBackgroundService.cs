using System.Collections.Generic;
using System.Diagnostics;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Attempts.SubmitAttempt;
using ExaminationSystem.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ExaminationSystem.API.Background;

public sealed class AttemptDeadlineBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<AttemptAutoSubmitOptions> options,
    AttemptAutoSubmitMetrics metrics,
    IDateTimeProvider clock,
    ILogger<AttemptDeadlineBackgroundService> logger) : BackgroundService
{
    private DateTime? _stuckSubmittingPresenceStartedUtc;
    private DateTime? _lastAlertLoggedAt;
    private DateTime? _lastWarningLoggedAt;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var poll = TimeSpan.FromSeconds(Math.Clamp(options.Value.PollIntervalSeconds, 5, 300));

        while (!stoppingToken.IsCancellationRequested)
        {
            var cycleSw = Stopwatch.StartNew();
            var sweepId = Guid.NewGuid();
            var sweepTimestampUtc = clock.UtcNow;
            try
            {
                using (logger.BeginScope(new Dictionary<string, object>
                       {
                           ["SweepId"] = sweepId,
                           ["SweepTimestampUtc"] = sweepTimestampUtc
                       }))
                {
                    var sweep = await RunSweepAsync(sweepTimestampUtc, stoppingToken);

                    metrics.RecordSweep(
                        sweep.GlobalStuckSubmittingCount,
                        sweep.GlobalMaxStuckAgeSeconds,
                        sweep.ReclaimedRows,
                        sweep.Processed,
                        sweep.ClaimRows,
                        sweep.Failures,
                        sweep.Elapsed.TotalSeconds);

                    UpdateStuckPresenceAlert(
                        sweep.GlobalStuckSubmittingCount,
                        sweep.GlobalMaxStuckAgeSeconds);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Attempt auto-submit sweep failed after {ElapsedMs} ms.", cycleSw.ElapsedMilliseconds);
            }

            try
            {
                var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(0, 2000));
                await Task.Delay(poll + jitter, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private void UpdateStuckPresenceAlert(int globalStuckCount, double maxStuckAgeSeconds)
    {
        var now = clock.UtcNow;
        var opt = options.Value;
        var sustained = TimeSpan.FromMinutes(Math.Clamp(opt.StuckSubmittingAlertSustainedMinutes, 1, 1440));
        var threshold = Math.Max(1, opt.StuckSubmittingAlertThresholdCount);
        var throttle = TimeSpan.FromSeconds(Math.Clamp(opt.AlertThrottleSeconds, 1, 86400));

        if (globalStuckCount <= 0)
        {
            _stuckSubmittingPresenceStartedUtc = null;
            _lastAlertLoggedAt = null;
            _lastWarningLoggedAt = null;
            return;
        }

        _stuckSubmittingPresenceStartedUtc ??= now;
        var sustainedElapsed = now - _stuckSubmittingPresenceStartedUtc.Value;

        if (ShouldLog(ref _lastWarningLoggedAt, now, throttle))
        {
            logger.LogWarning(
                "Stuck Submitting snapshot: globalCount={GlobalStuckCount}, maxAgeSeconds={MaxAgeSeconds}, timeoutMinutes={TimeoutMinutes}.",
                globalStuckCount,
                maxStuckAgeSeconds,
                opt.SubmittingTimeoutMinutes);
        }

        var alertCondition = globalStuckCount >= threshold || sustainedElapsed >= sustained;
        if (alertCondition && ShouldLog(ref _lastAlertLoggedAt, now, throttle))
        {
            logger.LogError(
                "ALERT: abnormal stuck Submitting workload. globalCount={GlobalStuckCount}, threshold={Threshold}, sustainedMinutes={SustainedMinutes:F1}, maxAgeSeconds={MaxAgeSeconds}. Investigate workers or DB health.",
                globalStuckCount,
                threshold,
                sustainedElapsed.TotalMinutes,
                maxStuckAgeSeconds);
        }
    }

    private static bool ShouldLog(ref DateTime? lastLoggedAt, DateTime now, TimeSpan throttle)
    {
        if (lastLoggedAt is null || now - lastLoggedAt.Value >= throttle)
        {
            lastLoggedAt = now;
            return true;
        }

        return false;
    }

    private async Task<SweepStats> RunSweepAsync(DateTime sweepAsOfUtc, CancellationToken stoppingToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var submitClaim = scope.ServiceProvider.GetRequiredService<IQuizAttemptAutoSubmitClaim>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var opt = options.Value;
        var maxPerBatch = Math.Clamp(opt.MaxAttemptsPerTick, 1, 500);
        var maxRounds = Math.Clamp(opt.MaxSweepRoundsPerPoll, 1, 100);
        var submittingStuckThreshold = TimeSpan.FromMinutes(Math.Clamp(opt.SubmittingTimeoutMinutes, 1, 1440));
        var maxWorkloadPerSweep = opt.MaxAttemptsPerSweep <= 0
            ? int.MaxValue
            : Math.Clamp(opt.MaxAttemptsPerSweep, 1, 500_000);

        var totalProcessed = 0;
        var totalClaims = 0;
        var totalReclaims = 0;
        var totalFailures = 0;
        var stuckTouchedInBatch = 0;
        var sweepSw = Stopwatch.StartNew();
        var workloadExceeded = false;

        int WorkloadSum() => totalProcessed + totalClaims + totalReclaims + totalFailures;

        for (var round = 0; round < maxRounds && !workloadExceeded; round++)
        {
            stoppingToken.ThrowIfCancellationRequested();
            if (WorkloadSum() >= maxWorkloadPerSweep)
                break;

            var now = sweepAsOfUtc;
            var batch = await submitClaim.GetOverdueWorkItemsAsync(
                maxPerBatch,
                now,
                submittingStuckThreshold,
                stoppingToken);
            if (batch.Count == 0)
                break;

            stuckTouchedInBatch += batch.Count(x => x.RequiresStuckSubmittingReclaim);

            foreach (var item in batch)
            {
                stoppingToken.ThrowIfCancellationRequested();
                if (WorkloadSum() >= maxWorkloadPerSweep)
                {
                    workloadExceeded = true;
                    break;
                }

                try
                {
                    if (item.NeedsInProgressClaim)
                    {
                        var claimed = await submitClaim.TryClaimInProgressOverdueAsync(
                            item.AttemptId,
                            now,
                            stoppingToken);
                        totalClaims += claimed;
                        if (claimed == 0)
                            continue;
                    }

                    if (WorkloadSum() >= maxWorkloadPerSweep)
                    {
                        workloadExceeded = true;
                        break;
                    }

                    if (item.RequiresStuckSubmittingReclaim)
                    {
                        var reclaimed = await submitClaim.TryReclaimStuckSubmittingAsync(
                            item.AttemptId,
                            now,
                            submittingStuckThreshold,
                            stoppingToken);
                        totalReclaims += reclaimed;
                        if (reclaimed == 0)
                            continue;
                    }

                    if (WorkloadSum() >= maxWorkloadPerSweep)
                    {
                        workloadExceeded = true;
                        break;
                    }

                    var outcome = await mediator.Send(
                        new SubmitAttemptOrchestrator(item.AttemptId, item.UserId),
                        stoppingToken);

                    var finalized = outcome.Success
                        || outcome.Code == ResultCode.AttemptTimedOut
                        || outcome.Code == ResultCode.SubmitAttemptSuccessful;
                    if (finalized)
                    {
                        totalProcessed++;
                    }
                    else if (outcome.Code != ResultCode.AttemptAlreadySubmitted)
                    {
                        totalFailures++;
                        logger.LogDebug(
                            "Auto-submit outcome for {AttemptId}: success={Success}, code={Code}.",
                            item.AttemptId,
                            outcome.Success,
                            outcome.Code);
                    }
                }
                catch (Exception ex)
                {
                    totalFailures++;
                    logger.LogWarning(ex, "Auto-submit failed for attempt {AttemptId}.", item.AttemptId);
                }
            }

            if (batch.Count < maxPerBatch || workloadExceeded)
                break;
        }

        var asOfUtc = sweepAsOfUtc;
        var globalStuckCount = await submitClaim.GetTotalStuckSubmittingCountAsync(
            asOfUtc,
            submittingStuckThreshold,
            stoppingToken);
        var globalMaxAge = globalStuckCount > 0
            ? await submitClaim.GetMaxStuckSubmittingAgeSecondsAsync(
                  asOfUtc,
                  submittingStuckThreshold,
                  stoppingToken) ?? 0d
            : 0d;

        if (totalProcessed > 0 || totalFailures > 0 || totalClaims > 0 || totalReclaims > 0
            || stuckTouchedInBatch > 0 || globalStuckCount > 0 || workloadExceeded)
        {
            logger.LogInformation(
                "Auto-submit sweep: processed={Processed}, claims={Claims}, reclaims={Reclaims}, stuckTouchedInBatch={StuckTouchedInBatch}, globalStuck={GlobalStuck}, workloadCapped={WorkloadCapped}, failures={Failures}, durationMs={DurationMs}.",
                totalProcessed,
                totalClaims,
                totalReclaims,
                stuckTouchedInBatch,
                globalStuckCount,
                workloadExceeded,
                totalFailures,
                sweepSw.ElapsedMilliseconds);
        }

        return new SweepStats(
            totalProcessed,
            totalClaims,
            totalReclaims,
            totalFailures,
            stuckTouchedInBatch,
            globalStuckCount,
            globalMaxAge,
            sweepSw.Elapsed);
    }

    private readonly record struct SweepStats(
        int Processed,
        int ClaimRows,
        int ReclaimedRows,
        int Failures,
        int StuckTouchedInBatch,
        int GlobalStuckSubmittingCount,
        double GlobalMaxStuckAgeSeconds,
        TimeSpan Elapsed);
}
