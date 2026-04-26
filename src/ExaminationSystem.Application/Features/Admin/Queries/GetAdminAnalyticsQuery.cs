using ExaminationSystem.Application.Common.Results;
using Microsoft.Extensions.Caching.Memory;

namespace ExaminationSystem.Application.Features.Admin.Queries;

public sealed record GetAdminAnalyticsQuery(
    DateTime? From,
    DateTime? To,
    Guid? DiplomaId) : IQuery<RequestResult<GetAdminAnalyticsResponse>>;

public sealed class GetAdminAnalyticsQueryHandler(
    IRepository<QuizAttempt> quizAttemptRepository,
    IRepository<AttemptAnswer> attemptAnswerRepository,
    IMemoryCache cache)
    : IRequestHandler<GetAdminAnalyticsQuery, RequestResult<GetAdminAnalyticsResponse>>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private const decimal FailedQuestionThresholdPercent = 40m;

    public async Task<RequestResult<GetAdminAnalyticsResponse>> Handle(
        GetAdminAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.From.HasValue && request.To.HasValue && request.From.Value > request.To.Value)
        {
            return RequestResult<GetAdminAnalyticsResponse>.Failure(
                CreateEmptyResponse(),
                ResultCode.ValidationError);
        }

        var fromUtc = request.From?.ToUniversalTime();
        var toUtc = request.To?.ToUniversalTime();
        var cacheKey = BuildCacheKey(fromUtc, toUtc, request.DiplomaId);

        if (cache.TryGetValue(cacheKey, out GetAdminAnalyticsResponse? cached) && cached is not null)
        {
            return RequestResult<GetAdminAnalyticsResponse>.succeeded(
                cached,
                ResultCode.AdminAnalyticsRetrievedSuccessfully);
        }

        var scopedAttempts = quizAttemptRepository
            .GetAll()
            .AsNoTracking()
            .Where(a => a.Result != null);

        if (request.DiplomaId.HasValue)
        {
            var diplomaId = request.DiplomaId.Value;
            scopedAttempts = scopedAttempts.Where(a => a.Quiz.DiplomaId == diplomaId);
        }

        if (fromUtc.HasValue)
        {
            var from = fromUtc.Value;
            scopedAttempts = scopedAttempts.Where(a => (a.SubmittedAt ?? a.StartTime) >= from);
        }

        if (toUtc.HasValue)
        {
            var to = toUtc.Value;
            scopedAttempts = scopedAttempts.Where(a => (a.SubmittedAt ?? a.StartTime) <= to);
        }

        var passRateByQuiz = await scopedAttempts
            .GroupBy(a => new { a.QuizId, a.Quiz.Title })
            .Select(g => new PassRateByQuizItem(
                g.Key.QuizId,
                g.Key.Title,
                g.Count(),
                Math.Round(g.Average(a => a.Result!.Passed ? 100m : 0m), 2)))
            .OrderByDescending(x => x.PassRatePercent)
            .ThenBy(x => x.QuizTitle)
            .ToListAsync(cancellationToken);

        var avgScoreByDiploma = await scopedAttempts
            .GroupBy(a => new { a.Quiz.DiplomaId, a.Quiz.Diploma.Title })
            .Select(g => new AvgScoreByDiplomaItem(
                g.Key.DiplomaId,
                g.Key.Title,
                g.Count(),
                Math.Round(g.Average(a => a.Result!.Score), 2)))
            .OrderByDescending(x => x.AverageScore)
            .ThenBy(x => x.DiplomaTitle)
            .ToListAsync(cancellationToken);

        var attemptsOverTimeBuckets = await scopedAttempts
            .Select(a => a.SubmittedAt ?? a.StartTime)
            .GroupBy(d => new { d.Year, d.Month, d.Day })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.Day,
                AttemptsCount = g.Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ThenBy(x => x.Day)
            .ToListAsync(cancellationToken);

        var attemptsOverTime = attemptsOverTimeBuckets
            .Select(x => new AttemptsOverTimeItem(
                DateTime.SpecifyKind(new DateTime(x.Year, x.Month, x.Day, 0, 0, 0), DateTimeKind.Utc),
                x.AttemptsCount))
            .ToList();

        // Use an inner join against filtered attempts to avoid expensive correlated subqueries.
        var scopedAttemptIds = scopedAttempts.Select(a => a.Id);
        var topFailedQuestionsData = await attemptAnswerRepository
            .GetAll()
            .AsNoTracking()
            .Join(
                scopedAttemptIds,
                answer => answer.AttemptId,
                attemptId => attemptId,
                (answer, _) => answer)
            .GroupBy(answer => new { answer.QuestionId, answer.Question.Text })
            .Select(g => new
            {
                g.Key.QuestionId,
                g.Key.Text,
                TotalAnswers = g.Count(),
                CorrectAnswers = g.Count(a => a.SelectedOption.IsCorrect)
            })
            .ToListAsync(cancellationToken);

        var topFailedQuestions = topFailedQuestionsData
            .Select(x => new TopFailedQuestionItem(
                x.QuestionId,
                x.Text,
                x.TotalAnswers,
                x.CorrectAnswers,
                x.TotalAnswers == 0 ? 0m : Math.Round((decimal)x.CorrectAnswers / x.TotalAnswers * 100m, 2)))
            .Where(x => x.CorrectAnswerRatePercent < FailedQuestionThresholdPercent)
            .OrderBy(x => x.CorrectAnswerRatePercent)
            .ThenByDescending(x => x.TotalAnswers)
            .ToList();

        var response = new GetAdminAnalyticsResponse
        {
            PassRateByQuiz = passRateByQuiz,
            AvgScoreByDiploma = avgScoreByDiploma,
            AttemptsOverTime = attemptsOverTime,
            TopFailedQuestions = topFailedQuestions
        };

        cache.Set(cacheKey, response, CacheDuration);

        return RequestResult<GetAdminAnalyticsResponse>.succeeded(
            response,
            ResultCode.AdminAnalyticsRetrievedSuccessfully);
    }

    private static string BuildCacheKey(DateTime? fromUtc, DateTime? toUtc, Guid? diplomaId)
    {
        var fromKey = fromUtc?.ToString("O") ?? "none";
        var toKey = toUtc?.ToString("O") ?? "none";
        var diplomaKey = diplomaId?.ToString() ?? "none";
        return $"admin_analytics:{fromKey}:{toKey}:{diplomaKey}";
    }

    private static GetAdminAnalyticsResponse CreateEmptyResponse() => new();
}
