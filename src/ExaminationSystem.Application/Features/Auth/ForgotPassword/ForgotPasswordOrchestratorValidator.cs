using FluentValidation;

namespace ExaminationSystem.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordOrchestratorValidator : AbstractValidator<ForgotPasswordOrchestrator>
{
    public ForgotPasswordOrchestratorValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
