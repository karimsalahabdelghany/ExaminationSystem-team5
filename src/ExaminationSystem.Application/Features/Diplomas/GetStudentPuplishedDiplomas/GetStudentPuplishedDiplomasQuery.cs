using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Diplomas.GetStudentDiplomas;

public record GetStudentPuplishedDiplomasQuery
(Guid userId , int pageNumber, int pageSize) : IRequest<RequestResult<PaginationResult<GetStudentPuplishedDiplomasResponse>>>;

public class GetStudentPuplishedDiplomasQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetStudentPuplishedDiplomasQuery, RequestResult<PaginationResult<GetStudentPuplishedDiplomasResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<RequestResult<PaginationResult<GetStudentPuplishedDiplomasResponse>>> Handle(GetStudentPuplishedDiplomasQuery request, CancellationToken cancellationToken)
    {
        var _repository = _unitOfWork.Repository<Diploma>();
        var diplomasQuery = _repository.GetAll(d => d.Status == DiplomaStatus.Published
                                       && d.Enrollments.Any(e => e.UserId == request.userId))
                                      .Select(d => new GetStudentPuplishedDiplomasResponse(
                                          d.Id,
                                          d.Title,
                                          d.Description,
                                          d.QuizCount,
                                           d.Quizzes
                                            .SelectMany(q => q.QuizAttempts
                                                .Where(qa =>
                                                    qa.UserId == request.userId &&
                                                    qa.Status == QuizAttemptStatus.Submitted &&
                                                    qa.Result != null))
                                            .Average(qa => (decimal?)qa.Result.Score)
                                      ));
        var diplomas = await diplomasQuery.PaginateAsync(request.pageNumber, request.pageSize,cancellationToken);
        return RequestResult<PaginationResult<GetStudentPuplishedDiplomasResponse>>.succeeded(diplomas, ResultCode.DiplomasRetrievedSuccessfully);
    }
}
