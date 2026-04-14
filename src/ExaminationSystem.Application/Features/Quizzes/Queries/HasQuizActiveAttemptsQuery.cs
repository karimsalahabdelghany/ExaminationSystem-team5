using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using MediatR;

namespace ExaminationSystem.Application.Features.Quizzes.Queries;

public record HasQuizActiveAttemptsQuery(Guid QuizId) : IRequest<bool>;

public class HasQuizActiveAttemptsQueryHandler(
    IRepository<QuizAttempt> attemptRepository) : IRequestHandler<HasQuizActiveAttemptsQuery, bool>
{
    public async Task<bool> Handle(HasQuizActiveAttemptsQuery request, CancellationToken cancellationToken)
    {
        return await attemptRepository
            .ExistsAsync(a => a.QuizId == request.QuizId && a.Status == QuizAttemptStatus.InProgress);
    }
}
