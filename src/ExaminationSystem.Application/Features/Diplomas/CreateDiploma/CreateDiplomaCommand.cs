using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using Mapster;

namespace ExaminationSystem.Application.Features.Diplomas.CreateDiploma;

public record CreateDiplomaCommand
(string Title, string? Description, int Duration, int QuizCount)
: ICommand<CreateDiplomaResponse>;


public class CreateDiplomaCommandHandler(IRepository<Diploma> repository) : IRequestHandler<CreateDiplomaCommand, CreateDiplomaResponse>
{
    private readonly IRepository<Diploma> _repository = repository;

    public async Task<CreateDiplomaResponse> Handle(CreateDiplomaCommand request, CancellationToken cancellationToken)
    {
        var diploma = request.Adapt<Diploma>();
        var result =  _repository.Add(diploma);
        return result.Adapt<CreateDiplomaResponse>();
    }
}
