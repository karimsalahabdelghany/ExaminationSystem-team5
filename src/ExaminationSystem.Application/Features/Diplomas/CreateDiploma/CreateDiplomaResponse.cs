using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Diplomas.CreateDiploma;

public record CreateDiplomaResponse
(Guid Id,
    string Title,
    string? Description,
    int Duration,
    int QuizCount,
    DiplomaStatus Status
);
