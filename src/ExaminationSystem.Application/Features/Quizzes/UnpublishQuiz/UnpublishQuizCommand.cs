using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Quizzes.Queries;
using ExaminationSystem.Domain.Enums;
using Mapster;

namespace ExaminationSystem.Application.Features.Quizzes.UnpublishQuiz;

public record UnpublishQuizCommand(Guid QuizId) : ICommand<RequestResult<QuizResponse>>;

public class UnpublishQuizCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator) : IRequestHandler<UnpublishQuizCommand, RequestResult<QuizResponse>>
{
    public async Task<RequestResult<QuizResponse>> Handle(UnpublishQuizCommand request, CancellationToken cancellationToken)
    {
        var quizRepository = unitOfWork.Repository<Quiz>();

        var quiz = await quizRepository.GetByIdAsync(request.QuizId);

        var existsCheck = CheckQuizExists(quiz);
        if (existsCheck is not null) return existsCheck;

        var publishedCheck = CheckIsPublished(quiz!);
        if (publishedCheck is not null) return publishedCheck;

        var hasActiveAttempts = await mediator.Send(
            new HasQuizActiveAttemptsQuery(request.QuizId), cancellationToken);

        var activeAttemptsCheck = CheckNoActiveAttempts(hasActiveAttempts);
        if (activeAttemptsCheck is not null) return activeAttemptsCheck;

        quiz!.Status = QuizStatus.Draft;
        quizRepository.SaveInclude(quiz, nameof(quiz.Status));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResult<QuizResponse>.succeeded(
            quiz.Adapt<QuizResponse>(),
            ResultCode.QuizUnpublishedSuccessfully);
    }

    





    private static RequestResult<QuizResponse>? CheckQuizExists(Quiz? quiz)
    {
        return quiz is null
            ? RequestResult<QuizResponse>.Failure(null!, ResultCode.QuizNotFound)
            : null;
    }

    private static RequestResult<QuizResponse>? CheckIsPublished(Quiz quiz)
    {
        return quiz.Status != QuizStatus.Published
            ? RequestResult<QuizResponse>.Failure(null!, ResultCode.QuizAlreadyDraft)
            : null;
    }

    private static RequestResult<QuizResponse>? CheckNoActiveAttempts(bool hasActiveAttempts)
    {
        return hasActiveAttempts
            ? RequestResult<QuizResponse>.Failure(null!, ResultCode.QuizHasActiveAttempts)
            : null;
    }
}
