using ExaminationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    public record GetAdminAttemptDetailsResponse
    (
    Guid AttemptId,
    Guid StudentId,
    string StudentName,
    Guid QuizId,
    string QuizTitle,
    QuizAttemptStatus Status,
    DateTime StartTime,
    DateTime Deadline,
    DateTime? SubmittedAt,
    decimal? Score,
    bool? Passed,
    int? TotalQuestions,
    int? CorrectCount,
    List<QuestionBreakdownItem> PerQuestion
    );

    public record QuestionBreakdownItem
    (
    Guid QuestionId,
    string QuestionText,
    string? StudentAnswer,
    string CorrectAnswer,
    bool IsCorrect
    );
}
