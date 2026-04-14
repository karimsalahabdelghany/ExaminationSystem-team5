
namespace ExaminationSystem.Application.Features.Diplomas.GetStudentDiplomas;

public record GetStudentDiplomasResponse
(Guid Id, string Title, string? Description, int QuizCount , decimal StudentProgress);
