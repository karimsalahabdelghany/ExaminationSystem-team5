using ExaminationSystem.Application.Common.Results;
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
) : ICommand<RequestResult<QuizResponse>>;


public class CreateQuizCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<CreateQuizCommand, RequestResult<QuizResponse>>
{
    public async Task<RequestResult<QuizResponse>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = new Quiz(
            diplomaId: request.DiplomaId,
            title: request.Title,
            instructions: request.Instructions ?? string.Empty,
            durationMinutes: request.DurationMinutes,
            passScore: request.PassScore,
            maxAttempts: request.MaxAttempts,
            status: QuizStatus.Draft
        );

        unitOfWork.Repository<Quiz>().Add(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResult<QuizResponse>.succeeded(
            quiz.Adapt<QuizResponse>(),
            ResultCode.QuizCreatedSuccessfully);
    }
}
