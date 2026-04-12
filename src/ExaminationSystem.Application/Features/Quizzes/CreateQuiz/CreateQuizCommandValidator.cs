using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using FluentValidation;

namespace ExaminationSystem.Application.Features.Quizzes.CreateQuiz;

public class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizCommandValidator(IRepository<Diploma> diplomaRepository)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.DiplomaId)
            .NotEmpty().WithMessage("DiplomaId is required.")
            .MustAsync(async (id, _) =>
                await diplomaRepository.ExistsAsync(d => d.Id == id))
            .WithMessage("Diploma not found.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be a positive number.");

        RuleFor(x => x.PassScore)
            .InclusiveBetween(0, 100).WithMessage("Pass score must be between 0 and 100.");

        RuleFor(x => x.MaxAttempts)
            .GreaterThan(0).WithMessage("Max attempts must be a positive number.");
    }
}
