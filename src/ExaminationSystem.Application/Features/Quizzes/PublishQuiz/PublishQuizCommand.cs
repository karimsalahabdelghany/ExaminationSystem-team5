using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Quizzes.Queries;
using ExaminationSystem.Domain.Enums;
using Mapster;

namespace ExaminationSystem.Application.Features.Quizzes.PublishQuiz;

public record PublishQuizCommand(Guid QuizId) : ICommand<RequestResult<QuizResponse>>;

public class PublishQuizCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator) : IRequestHandler<PublishQuizCommand, RequestResult<QuizResponse>>
{
    public async Task<RequestResult<QuizResponse>> Handle(PublishQuizCommand request, CancellationToken cancellationToken)
    {
        var quizRepository = unitOfWork.Repository<Quiz>();

        var quiz = await quizRepository.GetByIdAsync(request.QuizId);

        var existsCheck = CheckQuizExists(quiz);
        if (existsCheck is not null) return existsCheck;

        var alreadyPublishedCheck = CheckNotAlreadyPublished(quiz!);
        if (alreadyPublishedCheck is not null) return alreadyPublishedCheck;

        var hasQuestions = await mediator.Send(
            new HasQuizQuestionsQuery(request.QuizId), cancellationToken);

        var questionsCheck = CheckHasQuestions(hasQuestions);
        if (questionsCheck is not null) return questionsCheck;

        quiz!.Status = QuizStatus.Published;
        quizRepository.SaveInclude(quiz, nameof(quiz.Status));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResult<QuizResponse>.succeeded(
            quiz.Adapt<QuizResponse>(),
            ResultCode.QuizPublishedSuccessfully);
    }







    private static RequestResult<QuizResponse>? CheckQuizExists(Quiz? quiz)
    {
        return quiz is null
            ? RequestResult<QuizResponse>.Failure(null!, ResultCode.QuizNotFound)
            : null;
    }

    private static RequestResult<QuizResponse>? CheckNotAlreadyPublished(Quiz quiz)
    {
        return quiz.Status == QuizStatus.Published
            ? RequestResult<QuizResponse>.Failure(null!, ResultCode.QuizAlreadyPublished)
            : null;
    }

    private static RequestResult<QuizResponse>? CheckHasQuestions(bool hasQuestions)
    {
        return !hasQuestions
            ? RequestResult<QuizResponse>.Failure(null!, ResultCode.QuizHasNoQuestions)
            : null;
    }
}
