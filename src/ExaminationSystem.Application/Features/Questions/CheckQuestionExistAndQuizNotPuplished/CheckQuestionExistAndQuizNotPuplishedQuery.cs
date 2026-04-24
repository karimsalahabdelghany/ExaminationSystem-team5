using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Questions.CheckQuestionExistAndQuizPuplished;

public record CheckQuestionExistAndQuizNotPuplishedQuery
(Guid QuestionId) : IQuery<RequestResult<bool>>;

public class CheckQuestionExistAndQuizNotPuplishedQueryHandler(IRepository<Question> questionRepository)
    : IRequestHandler<CheckQuestionExistAndQuizNotPuplishedQuery, RequestResult<bool>>
{
    public async Task<RequestResult<bool>> Handle(CheckQuestionExistAndQuizNotPuplishedQuery request, CancellationToken cancellationToken)
    {
        var isExist = await questionRepository.GetAll(q => q.Id == request.QuestionId 
                                                       && q.Quiz.Status != QuizStatus.Published)
                                              .AnyAsync(cancellationToken); 
        if(!isExist)
            return RequestResult<bool>.Failure(isExist, ResultCode.QuestionNotFoundOrQuizPublished);
        
        return RequestResult<bool>.succeeded(true, ResultCode.QuestionIsExistAndQuizPublished);
    }
}
