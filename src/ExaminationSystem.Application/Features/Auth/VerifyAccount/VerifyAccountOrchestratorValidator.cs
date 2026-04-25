using FluentValidation;

namespace ExaminationSystem.Application.Features.Auth.VerifyAccount;

public class VerifyAccountOrchestratorValidator : AbstractValidator<VerifyAccountOrchestrator>
{
    public VerifyAccountOrchestratorValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP is required.")
            .Length(6).WithMessage("OTP must be 6 characters long.");
    }
}
