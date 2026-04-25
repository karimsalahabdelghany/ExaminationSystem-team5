namespace ExaminationSystem.API.Background;

public sealed class AttemptAutoSubmitOptions
{
    public const string SectionName = "AttemptAutoSubmit";

    /// <summary>How often to scan for overdue in-progress attempts (seconds).</summary>
    public int PollIntervalSeconds { get; set; } = 15;

    /// <summary>Max attempts finalized per sweep.</summary>
    public int MaxAttemptsPerTick { get; set; } = 100;

    /// <summary>Max inner batches per poll (each batch loads up to <see cref="MaxAttemptsPerTick"/> ids).</summary>
    public int MaxSweepRoundsPerPoll { get; set; } = 25;

    /// <summary>
    /// Minimum age since claim markers (SubmittedAt, or UpdatedAt/CreatedAt when null) before a Submitting row
    /// without AttemptResult is treated as stuck and eligible for reclaim/work.
    /// </summary>
    public int SubmittingTimeoutMinutes { get; set; } = 10;

    /// <summary>Log error alert when stuck submitting count in a single sweep reaches or exceeds this value.</summary>
    public int StuckSubmittingAlertThresholdCount { get; set; } = 10;

    /// <summary>Log error alert when any stuck submitting persists this many minutes (wall clock across sweeps).</summary>
    public int StuckSubmittingAlertSustainedMinutes { get; set; } = 5;

    /// <summary>Minimum interval between repeated alert or snapshot logs while conditions persist (seconds).</summary>
    public int AlertThrottleSeconds { get; set; } = 60;

    /// <summary>
    /// Optional cap on combined claims, reclaims, and finalized submits per sweep (0 = unlimited).
    /// </summary>
    public int MaxAttemptsPerSweep { get; set; }

    /// <summary>Expose <c>GET /internal/attempts/stuck</c> (still requires API key when configured).</summary>
    public bool ExposeStuckDiagnosticsEndpoint { get; set; }

    /// <summary>When set, require header <c>X-Internal-Key</c> matching this value.</summary>
    public string? StuckDiagnosticsApiKey { get; set; }
}
