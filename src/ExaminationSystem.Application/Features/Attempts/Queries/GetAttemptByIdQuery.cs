using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using MediatR;

namespace ExaminationSystem.Application.Features.Attempts.Queries;

public record GetAttemptByIdQuery(Guid AttemptId) : IRequest<QuizAttempt?>;

public class GetAttemptByIdQueryHandler(
    IRepository<QuizAttempt> attemptRepository) : IRequestHandler<GetAttemptByIdQuery, QuizAttempt?>
{
    public async Task<QuizAttempt?> Handle(GetAttemptByIdQuery request, CancellationToken cancellationToken)
    {
        return await attemptRepository.GetByIdAsync(request.AttemptId);
    }
}
