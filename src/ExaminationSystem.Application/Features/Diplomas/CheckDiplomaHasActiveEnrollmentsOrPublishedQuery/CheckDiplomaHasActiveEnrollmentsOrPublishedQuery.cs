using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Application.Features.Diplomas.HasActiveEnrollmentsQuery;

public record CheckDiplomaHasActiveEnrollmentsOrPublishedQuery(Guid DiplomaId) : IRequest<RequestResult<bool>>;


public class HasActiveEnrollmentsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<CheckDiplomaHasActiveEnrollmentsOrPublishedQuery, RequestResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<RequestResult<bool>> Handle(CheckDiplomaHasActiveEnrollmentsOrPublishedQuery request, CancellationToken cancellationToken)
    {
        var _repository = _unitOfWork.Repository<Diploma>();
        var hasActiveEnrollmentsOrPublished = await _repository.GetAll(d => d.Id == request.DiplomaId && d.Status == DiplomaStatus.Published)
                                      .AnyAsync(d => d.Enrollments
                                                    .Any(e => e.Status == EnrollmentStatus.Active),
                                                    cancellationToken);
        return RequestResult<bool>.succeeded(hasActiveEnrollmentsOrPublished ,ResultCode.DiplomaHasActiveEnrollments);
    }
}
