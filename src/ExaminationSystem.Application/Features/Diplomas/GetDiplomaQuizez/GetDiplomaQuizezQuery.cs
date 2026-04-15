using ExaminationSystem.Application.Common.Exceptions;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Diplomas.CheckUserEnrollment;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Application.Features.Diplomas.GetDiplomaQuizez;

//TODO : Read Student id From Claims
public record GetDiplomaQuizezQuery
(Guid dipolmaId, Guid studentId) : IRequest<RequestResult<List<GetDiplomaQuizezResponse>>>;

public class GetDiplomaQuizezQueryHandler(IUnitOfWork unitOfWork ,IMediator mediator
    ) : IRequestHandler<GetDiplomaQuizezQuery, RequestResult<List<GetDiplomaQuizezResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;

    public async Task<RequestResult<List<GetDiplomaQuizezResponse>>> Handle(GetDiplomaQuizezQuery request, CancellationToken cancellationToken)
    {
        var isEnrolled = await _mediator.Send(new CheckUserEnrollmentQuery(request.studentId, request.dipolmaId), cancellationToken);
        if (!isEnrolled.Result)
            throw new ForbiddenException("You are not enrolled in this diploma.");
        var _repository = _unitOfWork.Repository<Diploma>();

        var diploma = await _repository.GetAll(d => d.Id == request.dipolmaId
                                              && d.Status == DiplomaStatus.Published
                                              && d.Enrollments.Any(e => e.UserId == request.studentId))
                                        .SelectMany(d => d.Quizzes
                                                         .Where(q => q.Status == QuizStatus.Published)
                                                         .Select(q => new GetDiplomaQuizezResponse
                                                                      (
                                                                          q.Id,
                                                                          q.Title,
                                                                          q.DurationMinutes,
                                                                          q.QuizAttempts.Count(qa => qa.UserId == request.studentId),
                                                                          q.QuizAttempts.Where(qa => qa.UserId == request.studentId)
                                                                                        .OrderByDescending(qa => qa.Result)
                                                                                        .Select(qa => qa.Result.Score)
                                                                                        .FirstOrDefault(),
                                                                          q.Status
                                                                      )
                                                         )
                                        ).ToListAsync(cancellationToken);


        if (diploma == null)
            throw new NotFoundException("Diploma not found or you are not enrolled in it.");

        return RequestResult<List<GetDiplomaQuizezResponse>>.succeeded(diploma , ResultCode.DiplomasRetrievedSuccessfully);
    }
}
