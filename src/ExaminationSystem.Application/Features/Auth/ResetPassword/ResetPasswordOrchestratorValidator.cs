using FluentValidation;
using ExaminationSystem.Application.Common.Validation;

namespace ExaminationSystem.Application.Features.Auth.ResetPassword;

public class ResetPasswordOrchestratorValidator : AbstractValidator<ResetPasswordOrchestrator>
{
    public ResetPasswordOrchestratorValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP is required.")
            .Length(6).WithMessage("OTP must be 6 characters long.");

        RuleFor(x => x.NewPassword)
            .StrongPassword("New password");
    }
}
