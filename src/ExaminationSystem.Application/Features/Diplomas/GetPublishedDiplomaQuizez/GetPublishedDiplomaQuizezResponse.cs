using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Diplomas.GetPublishedDiplomaQuizez;

public record  GetPublishedDiplomaQuizezResponse
(Guid Id , string Title ,int DurationMinutes , int AttemptCount , decimal LastScore , QuizStatus Status);
