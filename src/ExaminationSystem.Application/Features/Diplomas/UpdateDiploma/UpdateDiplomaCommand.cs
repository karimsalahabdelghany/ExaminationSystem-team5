using ExaminationSystem.Application.Common.Exceptions;
using ExaminationSystem.Application.Interfaces;
using Mapster;

namespace ExaminationSystem.Application.Features.Diplomas.UpdateDiploma;

public record UpdateDiplomaCommand
(Guid id , string Title, string? Description , int Duration, int QuizCount)
:ICommand<UpdateDiplomaResult>;

public class UpdateDiplomaCommandHandler(IRepository<Domain.Entities.Diploma> repository) : IRequestHandler<UpdateDiplomaCommand, UpdateDiplomaResult>
{
    private readonly IRepository<Domain.Entities.Diploma> _repository = repository;
    public async Task<UpdateDiplomaResult> Handle(UpdateDiplomaCommand request, CancellationToken cancellationToken)
    {
        var diploma = await _repository.GetByIdWithNoTracking(request.id);
                            
        if (diploma is null)
            throw new NotFoundException("Diploma not found");

        diploma.Title = request.Title;
        diploma.Description = request.Description;
        diploma.Duration = request.Duration;
        diploma.QuizCount = request.QuizCount;
        await _repository.PatchAsync(diploma ,cancellationToken, d => d.Title, d => d.Description, d => d.Duration, d => d.QuizCount);
        return request.Adapt<UpdateDiplomaResult>();
    }
}
