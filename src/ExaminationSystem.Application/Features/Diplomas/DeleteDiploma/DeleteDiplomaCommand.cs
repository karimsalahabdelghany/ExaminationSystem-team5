using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Diplomas.CheckDiplomaExist;
using ExaminationSystem.Application.Features.Diplomas.CheckDiplomaHasActiveEnrollmentsOrPublished;

namespace ExaminationSystem.Application.Features.Diplomas.DeleteDiploma;

public record DeleteDiplomaCommand(Guid Id) : ICommand<RequestResult<bool>>;

public class DeleteDiplomaCommandHandler(IUnitOfWork unitOfWork ,IMediator mediator) : IRequestHandler<DeleteDiplomaCommand, RequestResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;

    public async Task<RequestResult<bool>> Handle(DeleteDiplomaCommand request, CancellationToken cancellationToken)
    {
        var _repository = _unitOfWork.Repository<Diploma>();
        var isExist = await _mediator.Send(new CheckDiplomaExistQuery(request.Id), cancellationToken);
        if (!isExist.Result)
            return RequestResult<bool>.Failure(false, ResultCode.DiplomaNotFound);

        var hasActiveEnrollmentsOrPublished = await _mediator.Send(new CheckDiplomaHasActiveEnrollmentsOrPublishedQuery(request.Id), cancellationToken);
        if (hasActiveEnrollmentsOrPublished.Result)
            return RequestResult<bool>.Failure(false,ResultCode.DiplomaHasActiveEnrollmentsOrPublished);

        _repository.Delete(new Diploma
        {
            Id = request.Id
        });
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return RequestResult<bool>.succeeded(true, ResultCode.DiplomaDeletedSuccessfully);
    }
}

