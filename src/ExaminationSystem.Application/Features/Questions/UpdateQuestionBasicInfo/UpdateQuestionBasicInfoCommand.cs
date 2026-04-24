using ExaminationSystem.Application.Common.Results;

namespace ExaminationSystem.Application.Features.Questions.UpdateQuestionBasicInfo;

public record UpdateQuestionBasicInfoCommand
(Guid Id, string Text, string Explanation) : IQuery<RequestResult<bool>>;

public class UpdateQuestionBasicInfoCommandHandler(IRepository<Question> questionRepository, 
    ILogger<UpdateQuestionBasicInfoCommandHandler> logger)
    : IRequestHandler<UpdateQuestionBasicInfoCommand, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(UpdateQuestionBasicInfoCommand request, CancellationToken cancellationToken)
    {
        var question = new Question
        {
            Id = request.Id,
            Text = request.Text,
            Explanation = request.Explanation
        };

        questionRepository.SaveInclude(question,nameof(Question.Text), nameof(Question.Explanation));
        logger.LogInformation("Question with id {Id} updated successfully.", request.Id);
        return RequestResult<bool>.succeeded(true, ResultCode.QuestionBasicInfoUpdatedSuccessfully);
    }
}