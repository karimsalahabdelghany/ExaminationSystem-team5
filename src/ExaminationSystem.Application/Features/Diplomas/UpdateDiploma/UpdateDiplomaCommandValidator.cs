using FluentValidation;

namespace ExaminationSystem.Application.Features.Diplomas.UpdateDiploma;

public class UpdateDiplomaCommandValidator : AbstractValidator<UpdateDiplomaCommand>
{
    public UpdateDiplomaCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0.");

        RuleFor(x => x.QuizCount)
            .GreaterThanOrEqualTo(0).WithMessage("Quiz count must be greater than or equal to 0.");
    }
}
