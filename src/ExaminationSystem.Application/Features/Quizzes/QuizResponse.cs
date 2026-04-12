using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Quizzes;

public record QuizResponse(
    Guid Id,
    string Title,
    int DurationMinutes,
    int PassScore,
    int MaxAttempts,
    string? Instructions,
    QuizStatus Status,
    Guid DiplomaId,
    DateTime CreatedAt
);
