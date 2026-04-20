using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Attempts.GetStudentAttemptsHistory;

public record GetStudentAttemptHistoryResponse
(
  Guid AttempId,
  string QuizTitle,
  decimal? Score,
  QuizAttemptStatus Status,
  DateTime? SubmittedAt
);
