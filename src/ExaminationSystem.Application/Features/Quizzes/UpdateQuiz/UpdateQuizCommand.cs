using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Application.Common.Exceptions;
using ExaminationSystem.Domain.Entities;
using Mapster;

namespace ExaminationSystem.Application.Features.Quizzes.UpdateQuiz;

public record UpdateQuizCommand(
    Guid QuizId,
    string Title,
    int DurationMinutes,
    int PassScore,
    int MaxAttempts,
    string? Instructions
) : ICommand<QuizResponse>;


public class UpdateQuizCommandHandler(
    IRepository<Quiz> quizRepository) : IRequestHandler<UpdateQuizCommand, QuizResponse>
{
    public async Task<QuizResponse> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.GetByIdAsync(request.QuizId);

        if (quiz is null)
            throw new NotFoundException("Quiz", request.QuizId);

        quiz.Title = request.Title;
        quiz.DurationMinutes = request.DurationMinutes;
        quiz.PassScore = request.PassScore;
        quiz.MaxAttempts = request.MaxAttempts;
        quiz.Instructions = request.Instructions ?? string.Empty;

        quizRepository.Update(quiz);

        return quiz.Adapt<QuizResponse>();
    }
}
