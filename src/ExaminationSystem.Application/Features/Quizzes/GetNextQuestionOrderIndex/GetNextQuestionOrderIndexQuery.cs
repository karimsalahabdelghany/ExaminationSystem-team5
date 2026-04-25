using ExaminationSystem.Application.Common.Results;

namespace ExaminationSystem.Application.Features.Quizzes.GetNextQuestionOrderIndex;


public record GetNextQuestionOrderIndexQuery(Guid QuizId)
    : IQuery<RequestResult<int?>>;

public class GetNextQuestionOrderIndexQueryHandler(IRepository<Quiz> quizRepository)
    : IRequestHandler<GetNextQuestionOrderIndexQuery, RequestResult<int?>>
{
    public async Task<RequestResult<int?>> Handle(GetNextQuestionOrderIndexQuery request, CancellationToken cancellationToken)
    {
        var lastOrderIndex = await quizRepository
                                   .GetAll(q => q.Id == request.QuizId)
                                   .Select(q => new
                                   {
                                       IsExist = true,
                                       lastOrderIndex = q.Questions.Max(q => q.OrderIndex)
                                   })
                                  .FirstOrDefaultAsync(cancellationToken);


        if (lastOrderIndex is null)
            return RequestResult<int?>.Failure(null, ResultCode.QuizNotFound);

        return RequestResult<int?>.succeeded(lastOrderIndex.lastOrderIndex, ResultCode.QuizIsExist);
    }
}
