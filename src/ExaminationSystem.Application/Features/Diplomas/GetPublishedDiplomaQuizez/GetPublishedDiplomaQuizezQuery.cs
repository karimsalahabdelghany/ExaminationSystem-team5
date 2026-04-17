using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Diplomas.CheckUserEnrollment;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Diplomas.GetPublishedDiplomaQuizez;

//TODO : Read Student id From Claims
public record GetPublishedDiplomaQuizezQuery
(Guid dipolmaId, Guid studentId) : IRequest<RequestResult<List<GetPublishedDiplomaQuizezResponse>>>;

public class GetDiplomaQuizezQueryHandler(IUnitOfWork unitOfWork ,IMediator mediator
    ) : IRequestHandler<GetPublishedDiplomaQuizezQuery, RequestResult<List<GetPublishedDiplomaQuizezResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;

    public async Task<RequestResult<List<GetPublishedDiplomaQuizezResponse>>> Handle(GetPublishedDiplomaQuizezQuery request, CancellationToken cancellationToken)
    {
        var isEnrolled = await _mediator.Send(new CheckUserEnrollmentQuery(request.dipolmaId, request.studentId), cancellationToken);
        if (!isEnrolled.Result)
            return RequestResult<List<GetPublishedDiplomaQuizezResponse>>.Failure(null, ResultCode.StudentNotEnrolledInDiploma);

        var _repository = _unitOfWork.Repository<Diploma>();

        var diploma = await _repository.GetAll(d => d.Id == request.dipolmaId
                                              && d.Status == DiplomaStatus.Published
                                              && d.Enrollments.Any(e => e.UserId == request.studentId))
                                        .SelectMany(d => d.Quizzes
                                                         .Where(q => q.Status == QuizStatus.Published)
                                                         .Select(q => new GetPublishedDiplomaQuizezResponse
                                                                      (
                                                                          q.Id,
                                                                          q.Title,
                                                                          q.DurationMinutes,
                                                                          q.QuizAttempts.Count(qa => qa.UserId == request.studentId),
                                                                          q.QuizAttempts.Where(qa => qa.UserId == request.studentId)
                                                                                        .OrderByDescending(qa => qa.Result)
                                                                                        .Select(qa => qa.Result.Score)
                                                                                        .FirstOrDefault(),
                                                                          //q.Status
                                                                          q.QuizAttempts.Where(qa => qa.UserId == request.studentId)
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
