using FluentValidation;

namespace ExaminationSystem.Application.Features.Quizzes.UpdateQuiz;

public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizCommandValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty().WithMessage("QuizId is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be a positive number.");

        RuleFor(x => x.PassScore)
            .InclusiveBetween(0, 100).WithMessage("Pass score must be between 0 and 100.");

        RuleFor(x => x.MaxAttempts)
            .GreaterThan(0).WithMessage("Max attempts must be a positive number.");
    }
}
