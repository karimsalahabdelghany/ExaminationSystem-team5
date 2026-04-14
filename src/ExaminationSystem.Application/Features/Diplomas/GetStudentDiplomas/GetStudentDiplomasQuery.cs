using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;

namespace ExaminationSystem.Application.Features.Diplomas.GetStudentDiplomas;

public record GetStudentDiplomasQuery(Guid StudentId) : IRequest<RequestResult<GetStudentDiplomasResponse>>;

//public class GetStudentDiplomasQueryHandler(IRepository<Enrollment> repository) : IRequestHandler<GetStudentDiplomasQuery, RequestResult<GetStudentDiplomasResponse>>
//{
//    private readonly IRepository<Enrollment> _repository = repository;

//    public Task<RequestResult<GetStudentDiplomasResponse>> Handle(GetStudentDiplomasQuery request, CancellationToken cancellationToken)
//    {
//        var studentDiplomas = _repository.GetAll(e)
//    }
//}
