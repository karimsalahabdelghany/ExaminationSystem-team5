using ExaminationSystem.Application.Common.Results;

namespace ExaminationSystem.Application.Features.Diplomas.CheckUserEnrollment;

public record CheckUserEnrollmentQuery(Guid DiplomaId, Guid StudentId) : IRequest<RequestResult<bool>>;

public class CheckUserEnrollmentQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<CheckUserEnrollmentQuery, RequestResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<RequestResult<bool>> Handle(CheckUserEnrollmentQuery request, CancellationToken cancellationToken)
    {
        var _repository = _unitOfWork.Repository<Diploma>();
        var enrollmentExists = await _repository.GetAll(d => d.Id == request.DiplomaId)
                                                .AnyAsync(d => d.Enrollments.Any(e => e.UserId == request.StudentId) ,cancellationToken);

        if (!enrollmentExists)
            return RequestResult<bool>.Failure(false, ResultCode.StudentNotEnrolledInDiploma);

        return RequestResult<bool>.succeeded(true, ResultCode.StudentEnrolledInDiploma);
    }
}
