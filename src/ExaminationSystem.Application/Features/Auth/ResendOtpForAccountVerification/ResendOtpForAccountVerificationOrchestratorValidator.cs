using FluentValidation;

namespace ExaminationSystem.Application.Features.Auth.ResendOtpForAccountVerification;

public class ResendOtpForAccountVerificationOrchestratorValidator :AbstractValidator<ResendOtpForAccountVerificationOrchestrator>
{
    public ResendOtpForAccountVerificationOrchestratorValidator()
    {
        RuleFor(x => x.UserEmail).NotEmpty()
                                 .EmailAddress()
                                 .WithMessage("Email must Not Empty and valid email");
    }
}
