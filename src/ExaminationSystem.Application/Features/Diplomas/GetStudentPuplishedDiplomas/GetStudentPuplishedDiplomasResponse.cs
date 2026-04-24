namespace ExaminationSystem.Application.Features.Diplomas.GetStudentDiplomas;

public record GetStudentPuplishedDiplomasResponse(
    Guid Id,
    string Title,
    string? Description,
    int QuizCount,
    decimal? StudentProgress
    );
