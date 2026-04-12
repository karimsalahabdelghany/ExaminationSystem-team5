namespace ExaminationSystem.Application.Features.Diplomas.UpdateDiploma;

public record UpdateDiplomaResult
(Guid Id, string Name, string Description, int Duration, int QuizCount);
