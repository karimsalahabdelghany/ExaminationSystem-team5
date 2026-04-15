namespace ExaminationSystem.Application.Features.Diplomas.GetDiplomas;

public record GetDiplomaResponse(Guid Id,
    string Title,
    string? Description,
    int QuizCount,
    decimal? StudentProgress
);


public record GetDiplomasResponse(List<GetDiplomaResponse> Diplomas);