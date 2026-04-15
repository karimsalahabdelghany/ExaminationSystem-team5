using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Diplomas.GetDiplomaQuizez;

public record  GetDiplomaQuizezResponse
(Guid Id , string Title ,int DurationMinutes , int AttemptCount , decimal LastScore , QuizStatus Status);
