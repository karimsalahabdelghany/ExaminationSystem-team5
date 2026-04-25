using FluentValidation;

namespace ExaminationSystem.Application.Features.Admin.Commands.SetUserLockState;

public class SetUserLockStateCommandValidator : AbstractValidator<SetUserLockStateCommand>
{
    public SetUserLockStateCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");
    }
}
