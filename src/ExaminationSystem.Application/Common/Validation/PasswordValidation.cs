using FluentValidation;

namespace ExaminationSystem.Application.Common.Validation;

public static class PasswordValidation
{
    // At least one lowercase, one uppercase, one digit, one special char, 8-64 chars.
    public const string StrongPasswordPattern =
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,64}$";

    public static IRuleBuilderOptions<T, string> StrongPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string fieldName = "Password")
    {
        return ruleBuilder
            .NotEmpty().WithMessage($"{fieldName} is required.")
            .Matches(StrongPasswordPattern)
            .WithMessage(
                $"{fieldName} must be 8-64 chars and include uppercase, lowercase, number, and special character.");
    }
}
