using ExaminationSystem.Application.Common.Results;

namespace ExaminationSystem.Application.Features.Quizzes.DeleteDiplomaQuizzesByDiplomaId;

public record DeleteDiplomaQuizzesByDiplomaIdCommand(Guid DiplomaId) 
    : ICommand<RequestResult<bool>>;

public class DeleteDiplomaQuizzesByDiplomaIdCommandHandler(
    IRepository<Quiz> repository) : IRequestHandler<DeleteDiplomaQuizzesByDiplomaIdCommand, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(DeleteDiplomaQuizzesByDiplomaIdCommand request, CancellationToken cancellationToken)
    {
        var quizzes = await repository.GetAll(q => q.DiplomaId == request.DiplomaId)
                                      .Select(q => q.Id)
                                      .ToListAsync();
        foreach (var quizId in quizzes)
            repository.Delete(new Quiz (quizId));
        return RequestResult<bool>.succeeded(true,ResultCode.DiplomaQuizzesDeletedSuccessfully);
    }
}

