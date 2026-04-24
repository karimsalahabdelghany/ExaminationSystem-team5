using ExaminationSystem.Application.Common.Results;

namespace ExaminationSystem.Application.Features.Questions.CheckQuestionExist;

public record CheckQuestionExistQuery
(Guid QuestionId) : IQuery<RequestResult<bool>>;

public class CheckQuestionExistQueryHandler (IRepository<Question> questionRepository)
    : IRequestHandler<CheckQuestionExistQuery, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(CheckQuestionExistQuery request, CancellationToken cancellationToken)
    {
        var isExist = await questionRepository.GetAll(q => q.Id == request.QuestionId)
                                     .AnyAsync(cancellationToken); 
        if(!isExist)
            return RequestResult<bool>.Failure(isExist, ResultCode.QuestionNotFound);
        return RequestResult<bool>.succeeded(isExist, ResultCode.QuestionIsExist);
    }
}
