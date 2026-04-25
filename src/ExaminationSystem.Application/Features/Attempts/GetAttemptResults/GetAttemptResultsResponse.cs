using System.Text.Json.Serialization;

namespace ExaminationSystem.Application.Features.Attempts.GetAttemptResults;

public record GetAttemptResultsResponse(
    [property: JsonPropertyName("score")] decimal Score,
    [property: JsonPropertyName("passed")] bool Passed,
    [property: JsonPropertyName("total_questions")] int TotalQuestions,
    [property: JsonPropertyName("correct_count")] int CorrectCount,
    [property: JsonPropertyName("per_question")] IReadOnlyList<AttemptQuestionResultResponse> PerQuestion
);

public record AttemptQuestionResultResponse(
    [property: JsonPropertyName("question_id")] Guid QuestionId,
    [property: JsonPropertyName("student_answer")] string? StudentAnswer,
    [property: JsonPropertyName("correct_answer")] string? CorrectAnswer,
    [property: JsonPropertyName("is_correct")] bool IsCorrect,
    [property: JsonPropertyName("explanation")] string? Explanation
);
