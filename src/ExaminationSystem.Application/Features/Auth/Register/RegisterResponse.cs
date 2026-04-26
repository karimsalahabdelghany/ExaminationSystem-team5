using FluentValidation;
using ExaminationSystem.Application.Common.Validation;

namespace ExaminationSystem.Application.Features.Auth.Register
{
    public record RegisterResponse(Guid UserId, List<string> Errors = null);
    public class RegisterOrchestratorValidator : AbstractValidator<RegisterOrchestrator>
    {
        public RegisterOrchestratorValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters");

            RuleFor(x => x.Password)
                .StrongPassword("Password");
        }

    }
}
