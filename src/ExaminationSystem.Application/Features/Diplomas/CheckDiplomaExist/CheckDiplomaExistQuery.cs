using ExaminationSystem.Application.Common.Results;

namespace ExaminationSystem.Application.Features.Diplomas.CheckDiplomaExist;

public record CheckDiplomaExistQuery(Guid DiplomaId) : IRequest<RequestResult<bool>>;

public class CheckDiplomaExistQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<CheckDiplomaExistQuery, RequestResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<RequestResult<bool>> Handle(CheckDiplomaExistQuery request, CancellationToken cancellationToken)
    {
        var _repository = _unitOfWork.Repository<Diploma>();
        var diplomaExists = await _repository.GetAll(d => d.Id == request.DiplomaId)
                                            .AnyAsync(cancellationToken);
        return RequestResult<bool>.succeeded(diplomaExists, ResultCode.DiplomaExist);
    }
}