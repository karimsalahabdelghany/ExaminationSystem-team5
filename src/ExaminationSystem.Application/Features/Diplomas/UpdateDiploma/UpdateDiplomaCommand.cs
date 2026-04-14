using ExaminationSystem.Application.Common.Exceptions;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Application.Features.Diplomas.UpdateDiploma;

public record UpdateDiplomaCommand
(Guid id , string Title, string? Description , int Duration, int QuizCount)
:ICommand<RequestResult<UpdateDiplomaResult>>;

public class UpdateDiplomaCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateDiplomaCommand, RequestResult<UpdateDiplomaResult>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<RequestResult<UpdateDiplomaResult>> Handle(UpdateDiplomaCommand request, CancellationToken cancellationToken)
    {
        var _repository = _unitOfWork.Repository<Diploma>();
        var diploma = await _repository.GetAll(d => d.Id == request.id).
                                       FirstOrDefaultAsync(cancellationToken);

        if (diploma is null)
            throw new NotFoundException("Diploma not found");

        diploma.Title = request.Title;
        diploma.Description = request.Description;
        diploma.Duration = request.Duration;
        diploma.QuizCount = request.QuizCount;
        _repository.SaveInclude(diploma, nameof(diploma.Title), nameof(diploma.Description), nameof(diploma.Duration), nameof(diploma.QuizCount));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return RequestResult<UpdateDiplomaResult>.succeeded(diploma.Adapt<UpdateDiplomaResult>(), ResultCode.DiplomaUpdatedSuccessfully);
    }
}
