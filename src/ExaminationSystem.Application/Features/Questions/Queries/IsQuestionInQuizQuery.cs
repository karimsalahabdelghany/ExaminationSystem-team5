using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using MediatR;

namespace ExaminationSystem.Application.Features.Questions.Queries;

public record IsQuestionInQuizQuery(Guid QuestionId, Guid QuizId) : IRequest<bool>;

public class IsQuestionInQuizQueryHandler(
    IRepository<Question> questionRepository) : IRequestHandler<IsQuestionInQuizQuery, bool>
{
    public async Task<bool> Handle(IsQuestionInQuizQuery request, CancellationToken cancellationToken)
    {
        return await questionRepository
            .ExistsAsync(q => q.Id == request.QuestionId && q.QuizId == request.QuizId);
    }
}
