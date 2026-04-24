using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Services;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Diplomas.GetStudentDiplomas;

public record GetStudentPublishedDiplomasQuery(
    PaginationParams Params
) : IRequest<RequestResult<PaginatedResult<GetStudentPuplishedDiplomasResponse>>>;

public class GetStudentPublishedDiplomasQueryHandler
    : IRequestHandler<GetStudentPublishedDiplomasQuery, RequestResult<PaginatedResult<GetStudentPuplishedDiplomasResponse>>>
{
    private readonly IRepository<Diploma> _diplomaRepo;
    private readonly ICurrentUser _currentUser;

    public GetStudentPublishedDiplomasQueryHandler(
        IRepository<Diploma> diplomaRepo,
        ICurrentUser currentUser)
    {
        _diplomaRepo = diplomaRepo;
        _currentUser = currentUser;
    }

    public async Task<RequestResult<PaginatedResult<GetStudentPuplishedDiplomasResponse>>> Handle(
        GetStudentPublishedDiplomasQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _diplomaRepo
            .GetAll()
            .AsNoTracking()
            .Where(d => d.Status == DiplomaStatus.Published
                     && !d.IsDeleted
                     && d.Enrollments.Any(e => e.UserId == _currentUser.Id))
            .Select(d => new GetStudentPuplishedDiplomasResponse(
                Id: d.Id,
                Title: d.Title,
                Description: d.Description,

                QuizCount: d.Quizzes.Count(q =>
                    q.Status == QuizStatus.Published
                 && !q.IsDeleted),

                StudentProgress: d.Quizzes
                    .SelectMany(q => q.QuizAttempts
                        .Where(qa =>
                            qa.UserId == _currentUser.Id
                         && qa.Status == QuizAttemptStatus.Submitted
                         && qa.Result != null))
                    .Average(qa => (decimal?)qa.Result!.Score)
            ))
            .ToPagedAsync(request.Params, cancellationToken);

        return RequestResult<PaginatedResult<GetStudentPuplishedDiplomasResponse>>
            .succeeded(result, ResultCode.DiplomasRetrievedSuccessfully);
    }
}