using ExaminationSystem.Application.Common.Exceptions;
using ExaminationSystem.Application.Features.Quizzes.Queries;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Quizzes.DeleteQuiz;

public record DeleteQuizCommand(Guid QuizId) : ICommand<Unit>;


public class DeleteQuizCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator) : IRequestHandler<DeleteQuizCommand, Unit>
{
    public async Task<Unit> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quizRepository = unitOfWork.Repository<Quiz>();

        var quiz = await quizRepository.GetByIdAsync(request.QuizId);

        if (quiz is null)
            throw new NotFoundException("Quiz", request.QuizId);

        if (quiz.Status == QuizStatus.Published)
            throw new ConflictException("Quiz", "Cannot delete a published quiz. Unpublish it first.");

        var hasActiveAttempts = await mediator.Send(
            new HasQuizActiveAttemptsQuery(request.QuizId), cancellationToken);

        if (hasActiveAttempts)
            throw new ConflictException("Quiz", "Cannot delete quiz while students have in-progress attempts.");

        quizRepository.Delete(quiz);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
