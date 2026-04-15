using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Diplomas.GetDiplomas;

//TODO : Replace StudentId with UserId From Claims and make sure that only students can access this endpoint
public record GetDiplomasQuery(Guid StudentId , int? PageNumber, int? PageSize) : IRequest<RequestResult<PaginationResult<GetDiplomaResponse>>>;

public class GetDiplomasQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDiplomasQuery, RequestResult<PaginationResult<GetDiplomaResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<RequestResult<PaginationResult<GetDiplomaResponse>>> Handle(GetDiplomasQuery request, CancellationToken cancellationToken)
    {
        var _repository = _unitOfWork.Repository<Diploma>();

        var diplomasQuery = _repository.GetAll(d => d.Status == DiplomaStatus.Published)
            .Select(d => new GetDiplomaResponse(
                d.Id,
                d.Title,
                d.Description,
                d.QuizCount,
                 d.Quizzes
                  .SelectMany(q => q.QuizAttempts
                      .Where(qa =>
                          qa.UserId == request.StudentId &&
                          qa.Status == QuizAttemptStatus.Submitted &&
                          qa.Result != null))
                  .Average(qa => (decimal?)qa.Result.Score)
            ));
        var diplomas = await diplomasQuery.PaginateAsync(request.PageNumber,request.PageSize ,cancellationToken);
        return  RequestResult<PaginationResult<GetDiplomaResponse>>.succeeded(diplomas, ResultCode.DiplomasRetrievedSuccessfully);
    }
}

