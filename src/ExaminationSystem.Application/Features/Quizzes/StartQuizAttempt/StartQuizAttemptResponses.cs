using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Quizzes.StartQuizAttempt;

public record StartQuizAttemptResponse(
    Guid AttemptId,
    DateTime StartTime,
    DateTime Deadline,
    StartQuizMetadataResponse Quiz,
    IReadOnlyCollection<StartQuizQuestionResponse> Questions
);

public record StartQuizMetadataResponse(
    Guid Id,
    string Title,
    string Instructions,
    int DurationMinutes,
    int PassScore,
    int MaxAttempts
);

public record StartQuizQuestionResponse(
    Guid Id,
    string Text,
    QuestionType Type,
    IReadOnlyCollection<StartQuizQuestionOptionResponse> Options
);

public record StartQuizQuestionOptionResponse(
    Guid Id,
    string Text
);

internal sealed record QuizStartProjection(
    Guid Id,
    string Title,
    string Instructions,
    int DurationMinutes,
    int PassScore,
    int MaxAttempts,
    IReadOnlyCollection<QuestionStartProjection> Questions);

internal sealed record QuestionStartProjection(
    Guid Id,
    string Text,
    QuestionType Type,
    IReadOnlyCollection<QuestionOptionStartProjection> Options);

internal sealed record QuestionOptionStartProjection(Guid Id, string Text);
