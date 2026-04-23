using ExaminationSystem.Application.Common.Results;

namespace ExaminationSystem.Application.Features.Attempts.GetAttempDetails;

public record GetAttempDetailsQuery
(Guid Id) : IQuery<RequestResult<GetAttempDetailsResponse>>;

public record GetAttempDetailsResponse(Guid Id , decimal? Score , bool Passed 
    , int TotalQuestions , int CorrectCount);

public class GetAttempDetailsQueryHandler (IRepository<AttemptResult> _repository)
    : IRequestHandler<GetAttempDetailsQuery, RequestResult<GetAttempDetailsResponse>>
{
    public async Task<RequestResult<GetAttempDetailsResponse>> Handle(GetAttempDetailsQuery request, CancellationToken cancellationToken)
    {
        var attempDetails = await _repository.GetAll(a => a.AttemptId == request.Id)
                                             .Select(a => new GetAttempDetailsResponse
                                              (a.Id , a.Score , a.Passed , a.TotalQuestions
                                              , a.CorrectCount))
                                             .FirstOrDefaultAsync(cancellationToken);
        if (attempDetails is null)
            return RequestResult<GetAttempDetailsResponse>.Failure(null,ResultCode.AttemptNotFound);

        return RequestResult<GetAttempDetailsResponse>.succeeded(attempDetails,ResultCode.AttemptDetailsRetrievedSuccessfully);
    }
}




