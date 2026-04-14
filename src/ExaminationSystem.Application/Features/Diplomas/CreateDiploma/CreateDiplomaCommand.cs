using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using Mapster;

namespace ExaminationSystem.Application.Features.Diplomas.CreateDiploma;

public record CreateDiplomaCommand
(string Title, string? Description, int Duration, int QuizCount)
: ICommand<RequestResult<CreateDiplomaResponse>>;


public class CreateDiplomaCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateDiplomaCommand, RequestResult<CreateDiplomaResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<RequestResult<CreateDiplomaResponse>> Handle(CreateDiplomaCommand request, CancellationToken cancellationToken)
    {
        var diploma = request.Adapt<Diploma>();
        var result =  _unitOfWork.Repository<Diploma>().Add(diploma);
        if(result != null) 
            await _unitOfWork.SaveChangesAsync();
        return RequestResult<CreateDiplomaResponse>.succeeded(result.Adapt<CreateDiplomaResponse>(), ResultCode.DiplomaCreatedSuccessfully);
    }
}
