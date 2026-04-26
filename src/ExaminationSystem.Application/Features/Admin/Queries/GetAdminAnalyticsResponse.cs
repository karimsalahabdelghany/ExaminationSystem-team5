using System.Text.Json.Serialization;

namespace ExaminationSystem.Application.Features.Admin.Queries;

public sealed class GetAdminAnalyticsResponse
{
    [JsonPropertyName("pass_rate_by_quiz")]
    public IReadOnlyList<PassRateByQuizItem> PassRateByQuiz { get; init; } = [];

    [JsonPropertyName("avg_score_by_diploma")]
    public IReadOnlyList<AvgScoreByDiplomaItem> AvgScoreByDiploma { get; init; } = [];

    [JsonPropertyName("attempts_over_time")]
    public IReadOnlyList<AttemptsOverTimeItem> AttemptsOverTime { get; init; } = [];

    [JsonPropertyName("top_failed_questions")]
    public IReadOnlyList<TopFailedQuestionItem> TopFailedQuestions { get; init; } = [];
}

public sealed record PassRateByQuizItem(
    Guid QuizId,
    string QuizTitle,
    int TotalAttempts,
    decimal PassRatePercent);

public sealed record AvgScoreByDiplomaItem(
    Guid DiplomaId,
    string DiplomaTitle,
    int AttemptCount,
    decimal AverageScore);

public sealed record AttemptsOverTimeItem(
    DateTime TimestampUtc,
    int AttemptsCount);

public sealed record TopFailedQuestionItem(
    Guid QuestionId,
    string QuestionText,
    int TotalAnswers,
    int CorrectAnswers,
    decimal CorrectAnswerRatePercent);
