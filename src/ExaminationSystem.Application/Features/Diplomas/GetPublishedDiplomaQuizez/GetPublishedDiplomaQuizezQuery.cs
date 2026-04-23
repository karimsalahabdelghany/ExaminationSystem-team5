using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Diplomas.CheckUserEnrollment;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Diplomas.GetPublishedDiplomaQuizez;

//TODO : Read Student id From Claims
public record GetPublishedDiplomaQuizezQuery
(Guid dipolmaId) : IRequest<RequestResult<List<GetPublishedDiplomaQuizezResponse>>>;

public class GetDiplomaQuizezQueryHandler(IRepository<Diploma> diplomaRepo, IMediator mediator , ICurrentUser currentUser
    ) : IRequestHandler<GetPublishedDiplomaQuizezQuery, RequestResult<List<GetPublishedDiplomaQuizezResponse>>>
{
    private readonly IRepository<Diploma> _diplomaRepo = diplomaRepo;
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<RequestResult<List<GetPublishedDiplomaQuizezResponse>>> Handle(GetPublishedDiplomaQuizezQuery request, CancellationToken cancellationToken)
    {
        var isEnrolled = await _mediator.Send(new CheckUserEnrollmentQuery(request.dipolmaId,_currentUser.Id.Value), cancellationToken);
        if (!isEnrolled.Result)
            return RequestResult<List<GetPublishedDiplomaQuizezResponse>>.Failure(null, ResultCode.StudentNotEnrolledInDiploma);

        //var _repository = _unitOfWork.Repository<Diploma>();

        var diploma = await _diplomaRepo.GetAll(d => d.Id == request.dipolmaId
                                              && d.Status == DiplomaStatus.Published
                                              && d.Enrollments.Any(e => e.UserId == _currentUser.Id))
                                        .SelectMany(d => d.Quizzes
                                                         .Where(q => q.Status == QuizStatus.Published)
                                                         .Select(q => new GetPublishedDiplomaQuizezResponse
                                                                      (
                                                                          q.Id,
                                                                          q.Title,
                                                                          q.DurationMinutes,
                                                                          q.QuizAttempts.Count(qa => qa.UserId == _currentUser.Id),
                                                                          q.QuizAttempts.Where(qa => qa.UserId == _currentUser.Id)
                                                                                        .OrderByDescending(qa => qa.Result)
                                                                                        .Select(qa => qa.Result.Score)
                                                                                        .FirstOrDefault(),
                                                                          //q.Status
                                                                          q.QuizAttempts.Where(qa => qa.UserId == _currentUser.Id)
                                                                                        .OrderByDescending(qa => qa.CreatedAt)
                                                                                        .Select(qa => qa.Quiz.Status)
                                                                                        .FirstOrDefault()
                                                                      )
                                                         )
                                        ).ToListAsync(cancellationToken);


        if (diploma == null)
            return RequestResult<List<GetPublishedDiplomaQuizezResponse>>.Failure(null, ResultCode.DiplomaNotFound);

        return RequestResult<List<GetPublishedDiplomaQuizezResponse>>.succeeded(diploma , ResultCode.DiplomasRetrievedSuccessfully);
    }
}
