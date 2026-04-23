namespace ExaminationSystem.Application.Features.Quizzes.Queries;

public record HasQuizQuestionsQuery(Guid QuizId) : IRequest<bool>;

public class HasQuizQuestionsQueryHandler(
    IRepository<Question> questionRepository) : IRequestHandler<HasQuizQuestionsQuery, bool>
{
    public async Task<bool> Handle(HasQuizQuestionsQuery request, CancellationToken cancellationToken)
    {
        return await questionRepository
            .ExistsAsync(q => q.QuizId == request.QuizId);
    }
}
