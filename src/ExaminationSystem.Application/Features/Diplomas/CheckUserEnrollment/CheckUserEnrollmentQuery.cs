using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Application.Features.Diplomas.CheckUserEnrollment;

public record CheckUserEnrollmentQuery(Guid DiplomaId, Guid StudentId) : IRequest<RequestResult<bool>>;

public class CheckUserEnrollmentQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<CheckUserEnrollmentQuery, RequestResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<RequestResult<bool>> Handle(CheckUserEnrollmentQuery request, CancellationToken cancellationToken)
    {
        var _repository = _unitOfWork.Repository<Diploma>();
        var enrollmentExists = await _repository.GetAll(d => d.Id == request.DiplomaId
                                                          && d.Enrollments
                                                              .Any(e => e.UserId == request.StudentId))
                                                .AnyAsync(cancellationToken);

        if (!enrollmentExists)
            return RequestResult<bool>.Failure(false, ResultCode.UserNotEnrolledInDiploma);

        return RequestResult<bool>.succeeded(true, ResultCode.UserEnrolledInDiploma);
    }
}
