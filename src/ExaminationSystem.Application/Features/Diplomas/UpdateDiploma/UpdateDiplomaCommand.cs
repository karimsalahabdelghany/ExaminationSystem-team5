using ExaminationSystem.Application.Common.Exceptions;
using ExaminationSystem.Application.Interfaces;

namespace ExaminationSystem.Application.Features.Diplomas.UpdateDiploma;

public record UpdateDiplomaCommand
(Guid id , string Name, string Description , int Duration, int QuizCount)
:ICommand<UpdateDiplomaResult>;

public class UpdateDiplomaCommandHandler(IRepository<Domain.Entities.Diploma> repository) : IRequestHandler<UpdateDiplomaCommand, UpdateDiplomaResult>
{
    private readonly IRepository<Domain.Entities.Diploma> _repository = repository;
    public async Task<UpdateDiplomaResult> Handle(UpdateDiplomaCommand request, CancellationToken cancellationToken)
    {
        var diploma = await _repository.GetByIdWithNoTracking(request.id);
                            
        if (diploma is null)
            throw new NotFoundException("Diploma not found");
        diploma.Title = request.Name;
        diploma.Description = request.Description;
        diploma.Duration = request.Duration;
        diploma.QuizCount = request.QuizCount;
        _repository.p
        return new UpdateDiplomaResult(result.Id, result.Title, result.Description ?? string.Empty, result.Duration, result.QuizCount);
    }
}
