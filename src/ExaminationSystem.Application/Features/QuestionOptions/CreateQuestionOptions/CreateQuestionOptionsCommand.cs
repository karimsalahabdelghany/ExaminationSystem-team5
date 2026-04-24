using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Questions.CreateQuestion;

namespace ExaminationSystem.Application.Features.QuestionOptions.CreateQuestionOptions;

public record CreateQuestionOptionsCommand
(Guid QuestionId, List<QuestionOptionRequest> Options)
    : IQuery<RequestResult<bool>>;

public class CreateQuestionOptionsCommandHandler(IRepository<QuestionOption> repository
              , ILogger<CreateQuestionOptionsCommandHandler> logger)
    : IRequestHandler<CreateQuestionOptionsCommand, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(CreateQuestionOptionsCommand request, CancellationToken cancellationToken)
    {
        var options = request.Options.Select(o => new QuestionOption
        {
            Id = Guid.CreateVersion7(),
            Text = o.Text,
            IsCorrect = o.IsCorrect,
            QuestionId = request.QuestionId,
        }).ToList();
        repository.AddRange(options);

        logger.LogInformation("Options for question with id {Id} created successfully.", request.QuestionId);
        return RequestResult<bool>.succeeded(true, ResultCode.OptionCreatedSuccessfully);
    }
}
