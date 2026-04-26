using ExaminationSystem.Application.Common.Results;

namespace ExaminationSystem.Application.Features.Questions.DeleteDiplomaQuizQuestionsByDiplomaId;

public record DeleteDiplomaQuizQuestionsByDiplomaIdCommand(Guid DiplomaId) 
    :ICommand<RequestResult<bool>>;

public class DeleteDiplomaQuizQuestionsByDiplomaIdCommandHandler(
    IRepository<Question> repository) : IRequestHandler<DeleteDiplomaQuizQuestionsByDiplomaIdCommand, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(DeleteDiplomaQuizQuestionsByDiplomaIdCommand request, CancellationToken cancellationToken)
    {
        var questions = await repository.GetAll(q => q.Quiz.DiplomaId == request.DiplomaId)
                                        .Select(q => q.Id)
                                        .ToListAsync();
        foreach (var questionId in questions)
            repository.Delete(new Question{ Id = questionId });
        return RequestResult<bool>.succeeded(true, ResultCode.DiplomaQuizQuestionsDeletedSuccessfully);
    }
}

