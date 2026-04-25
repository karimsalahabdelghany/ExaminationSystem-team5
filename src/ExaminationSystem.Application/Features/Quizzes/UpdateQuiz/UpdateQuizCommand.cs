using ExaminationSystem.Application.Common.Results;
using Mapster;

namespace ExaminationSystem.Application.Features.Quizzes.UpdateQuiz;

public record UpdateQuizCommand(
    Guid QuizId,
    string Title,
    int DurationMinutes,
    int PassScore,
    int MaxAttempts,
    string? Instructions
) : ICommand<RequestResult<QuizResponse>>;


public class UpdateQuizCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateQuizCommand, RequestResult<QuizResponse>>
{
    public async Task<RequestResult<QuizResponse>> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var quizRepository = unitOfWork.Repository<Quiz>();

        var quiz = await quizRepository.GetByIdAsync(request.QuizId);

        var existsCheck = CheckQuizExists(quiz);
        if (existsCheck is not null) return existsCheck;

        quiz!.Title = request.Title;
        quiz.DurationMinutes = request.DurationMinutes;
        quiz.PassScore = request.PassScore;
        quiz.MaxAttempts = request.MaxAttempts;
        quiz.Instructions = request.Instructions ?? string.Empty;

        //quizRepository.Update(quiz);
        quizRepository.SaveInclude(quiz, nameof(quiz.Title), nameof(quiz.DurationMinutes),
            nameof(quiz.PassScore), nameof(quiz.MaxAttempts), nameof(quiz.Instructions));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResult<QuizResponse>.succeeded(
            quiz.Adapt<QuizResponse>(),
            ResultCode.QuizUpdatedSuccessfully);
    }



    private static RequestResult<QuizResponse>? CheckQuizExists(Quiz? quiz)
    {
        return quiz is null
            ? RequestResult<QuizResponse>.Failure(null!, ResultCode.QuizNotFound)
            : null;
    }
}
