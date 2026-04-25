using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Quizzes.Queries;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Quizzes.DeleteQuiz;

public record DeleteQuizCommand(Guid QuizId) : ICommand<RequestResult<bool>>;


public class DeleteQuizCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator) : IRequestHandler<DeleteQuizCommand, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quizRepository = unitOfWork.Repository<Quiz>();

        var quiz = await quizRepository.GetByIdAsync(request.QuizId);

        var existsCheck = CheckQuizExists(quiz);
        if (existsCheck is not null) return existsCheck;

        var publishedCheck = CheckQuizNotPublished(quiz!);
        if (publishedCheck is not null) return publishedCheck;

        var hasActiveAttempts = await mediator.Send(
            new HasQuizActiveAttemptsQuery(request.QuizId), cancellationToken);

        var activeAttemptsCheck = CheckNoActiveAttempts(hasActiveAttempts);
        if (activeAttemptsCheck is not null) return activeAttemptsCheck;

        quizRepository.Delete(quiz!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResult<bool>.succeeded(true, ResultCode.QuizDeletedSuccessfully);
    }



    private static RequestResult<bool>? CheckQuizExists(Quiz? quiz)
    {
        return quiz is null
            ? RequestResult<bool>.Failure(false, ResultCode.QuizNotFound)
            : null;
    }

    private static RequestResult<bool>? CheckQuizNotPublished(Quiz quiz)
    {
        return quiz.Status == QuizStatus.Published
            ? RequestResult<bool>.Failure(false, ResultCode.QuizIsPublished)
            : null;
    }

    private static RequestResult<bool>? CheckNoActiveAttempts(bool hasActiveAttempts)
    {
        return hasActiveAttempts
            ? RequestResult<bool>.Failure(false, ResultCode.QuizHasActiveAttempts)
            : null;
    }
}
