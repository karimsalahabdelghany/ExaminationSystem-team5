using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Diplomas.CheckUserEnrollment;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Diplomas.GetPublishedDiplomaQuizez;


public record GetPublishedDiplomaQuizezQuery
(Guid dipolmaId, PaginationParams Params) : IRequest<RequestResult<List<PaginatedResult<GetPublishedDiplomaQuizezResponse>>>>;

public record GetPublishedDiplomaQuizzesQuery(
    Guid DiplomaId,
    PaginationParams Params
) : IRequest<RequestResult<PaginatedResult<GetPublishedDiplomaQuizezResponse>>>;

public class GetPublishedDiplomaQuizzesQueryHandler
    : IRequestHandler<GetPublishedDiplomaQuizzesQuery, RequestResult<PaginatedResult<GetPublishedDiplomaQuizezResponse>>>
{
    private readonly IRepository<Diploma> _diplomaRepo;
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public GetPublishedDiplomaQuizzesQueryHandler(
        IRepository<Diploma> diplomaRepo,
        IMediator mediator,
        ICurrentUser currentUser)
    {
        _diplomaRepo = diplomaRepo;
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public async Task<RequestResult<PaginatedResult<GetPublishedDiplomaQuizezResponse>>> Handle(
        GetPublishedDiplomaQuizzesQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.TryGetUserId(out var studentId))
            return RequestResult<PaginatedResult<GetPublishedDiplomaQuizezResponse>>
                .Failure(null, ResultCode.InvalidCredentials);

        // Step 1 — check enrollment first
        var isEnrolled = await _mediator.Send(
            new CheckUserEnrollmentQuery(request.DiplomaId, studentId),
            cancellationToken);

        if (!isEnrolled.Result)
            return RequestResult<PaginatedResult<GetPublishedDiplomaQuizezResponse>>
                .Failure(null, ResultCode.StudentNotEnrolledInDiploma);

        var result = await _diplomaRepo
            .GetAll(d => d.Id == request.DiplomaId
                      && d.Status == DiplomaStatus.Published
                      && !d.IsDeleted)
            .SelectMany(d => d.Quizzes
                .Where(q => q.Status == QuizStatus.Published
                         && !q.IsDeleted)
                .Select(q => new GetPublishedDiplomaQuizezResponse(
                    Id: q.Id,
                    Title: q.Title,
                    DurationMinutes: q.DurationMinutes,

                    AttemptCount: q.QuizAttempts
                        .Count(qa => qa.UserId == _currentUser.Id),

                    LastScore: q.QuizAttempts
                        .Where(qa => qa.UserId == _currentUser.Id
                                  && qa.Result != null)
                        .OrderByDescending(qa => qa.CreatedAt)
                        .Select(qa => (decimal?)qa.Result!.Score)
                        .FirstOrDefault(),

                    Status: q.Status
                )))
            .ToPagedAsync(request.Params, cancellationToken); 

        if (!result.Data.Any())
            return RequestResult<PaginatedResult<GetPublishedDiplomaQuizezResponse>>
                .Failure(null, ResultCode.DiplomaNotFound);

        return RequestResult<PaginatedResult<GetPublishedDiplomaQuizezResponse>>
            .succeeded(result, ResultCode.DiplomasRetrievedSuccessfully);
    }

}



