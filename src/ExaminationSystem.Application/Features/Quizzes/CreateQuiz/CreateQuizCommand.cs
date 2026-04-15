using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using Mapster;

namespace ExaminationSystem.Application.Features.Quizzes.CreateQuiz;

public record CreateQuizCommand(
    string Title,
    Guid DiplomaId,
    int DurationMinutes,
    int MaxAttempts,
    string? Instructions,
    int PassScore = 60
) : ICommand<QuizResponse>;


public class CreateQuizCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<CreateQuizCommand, QuizResponse>
{
    public async Task<QuizResponse> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var quizRepository = unitOfWork.Repository<Quiz>();

        var quiz = new Quiz(
            diplomaId: request.DiplomaId,
            title: request.Title,
            instructions: request.Instructions ?? string.Empty,
            durationMinutes: request.DurationMinutes,
            passScore: request.PassScore,
            maxAttempts: request.MaxAttempts,
            status: QuizStatus.Draft
        );

        quizRepository.Add(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return quiz.Adapt<QuizResponse>();
    }
}
