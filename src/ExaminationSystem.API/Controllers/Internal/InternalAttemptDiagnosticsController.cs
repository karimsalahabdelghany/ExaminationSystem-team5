using ExaminationSystem.API.Background;
using ExaminationSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ExaminationSystem.API.Controllers.Internal;

[ApiController]
[Route("internal/attempts")]
public sealed class InternalAttemptDiagnosticsController(
    IQuizAttemptAutoSubmitClaim submitClaim,
    IDateTimeProvider clock,
    IOptions<AttemptAutoSubmitOptions> options) : ControllerBase
{
    [HttpGet("stuck")]
    [ProducesResponseType(typeof(IReadOnlyList<StuckAttemptDiagnosticDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<StuckAttemptDiagnosticDto>>> GetStuckSubmittingAsync(
        CancellationToken cancellationToken)
    {
        var opt = options.Value;
        if (!opt.ExposeStuckDiagnosticsEndpoint)
            return NotFound();

        if (!string.IsNullOrEmpty(opt.StuckDiagnosticsApiKey))
        {
            if (!Request.Headers.TryGetValue("X-Internal-Key", out var key)
                || key.Count != 1
                || !string.Equals(key[0], opt.StuckDiagnosticsApiKey, StringComparison.Ordinal))
            {
                return Unauthorized();
            }
        }

        var threshold = TimeSpan.FromMinutes(Math.Clamp(opt.SubmittingTimeoutMinutes, 1, 1440));
        var now = clock.UtcNow;
        var rows = await submitClaim.GetStuckSubmittingDiagnosticsAsync(500, now, threshold, cancellationToken);
        var dto = rows
            .Select(r => new StuckAttemptDiagnosticDto(
                r.AttemptId,
                r.ClaimTimestamp,
                TimeSpan.FromSeconds(r.AgeSeconds)))
            .ToList();

        return Ok(dto);
    }
}

public sealed record StuckAttemptDiagnosticDto(Guid AttemptId, DateTime? SubmittedAt, TimeSpan Age);
