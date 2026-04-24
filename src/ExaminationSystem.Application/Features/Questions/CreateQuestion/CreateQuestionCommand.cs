using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Quizzes.GetNextQuestionOrderIndex;

namespace ExaminationSystem.Application.Features.Questions.CreateQuestion;

public record CreateQuestionCommand
(Guid QuizId, string Text, string Explanation,List<QuestionOptionRequest> Options)
    : ICommand<RequestResult<CreateQuestionResponse>>;

public record QuestionOptionRequest(string Text, bool IsCorrect);

public class CreateQuestionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, ILogger<CreateQuestionCommandHandler> logger)
    : IRequestHandler<CreateQuestionCommand, RequestResult<CreateQuestionResponse>>
{
    public async Task<RequestResult<CreateQuestionResponse>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var hasOneCorrectAnswer = CheckOptionsHasOnlyOneCorrectAnswer(request.Options);
        if (!hasOneCorrectAnswer)
            return RequestResult<CreateQuestionResponse>.Failure(null, ResultCode.QuestionHasMoreThanOneCorrectAnswer); 

        var quizeExistanceAndLastQuestionOrder = await mediator.Send(new GetNextQuestionOrderIndexQuery(request.QuizId),cancellationToken);

        if(quizeExistanceAndLastQuestionOrder.Code == ResultCode.QuizNotFound)
            return RequestResult<CreateQuestionResponse>.Failure(null, ResultCode.QuizNotFound);


        logger.LogInformation("Begain Create New Question in Quize {QuizeId}",request.QuizId);
        var newQuestionOrder = (quizeExistanceAndLastQuestionOrder?.Result ?? 0) + 1;
        var question = new Question
        {
            Id = Guid.CreateVersion7(),
            QuizId = request.QuizId,
            Text = request.Text,
            Explanation = request.Explanation,
            OrderIndex = newQuestionOrder,
            Options = request.Options.Select(o => new QuestionOption
            {
                Id = Guid.CreateVersion7(),
                Text = o.Text,
                IsCorrect = o.IsCorrect
            }).ToList()
        };

        var repository = unitOfWork.Repository<Question>();
        repository.Add(question);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create question for Quiz {QuizId}.", request.QuizId);
            return RequestResult<CreateQuestionResponse>.Failure(null, ResultCode.FailedToCreate);
        }

        logger.LogInformation(
          "Question {QuestionId} created successfully for Quiz {QuizId}.",
          question.Id, request.QuizId);

        return RequestResult<CreateQuestionResponse>
            .succeeded(new CreateQuestionResponse(question.Id),
                       ResultCode.QuestionCreatedSuccessfully);
    }

    private bool CheckOptionsHasOnlyOneCorrectAnswer(List<QuestionOptionRequest> options)
    {
        return options.Count(o => o.IsCorrect) == 1;
    }
}



