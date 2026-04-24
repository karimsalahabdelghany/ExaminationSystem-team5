using FluentValidation;

namespace ExaminationSystem.Application.Features.Questions.CreateQuestion;

public class CreateQuestionCommandValidator :AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(c => c.Text)
       .NotEmpty().WithMessage("Question text is required.")
       .MaximumLength(1000).WithMessage("Question text must not exceed 1000 characters.");

        RuleFor(c => c.Explanation)
            .MaximumLength(2000).WithMessage("Explanation must not exceed 2000 characters.");

        RuleForEach(c => c.Options)
            .ChildRules(option =>
            {
                option.RuleFor(o => o.Text)
                    .NotEmpty().WithMessage("Option text is required.");
            });

        RuleFor(c => c.Options)
             .NotEmpty()
             .NotNull().WithMessage("Options are required.")
             .Must(options => options.Count >= 2).WithMessage("A question must have at least two options.");
    }
}
