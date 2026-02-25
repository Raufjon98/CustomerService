using FluentValidation;

namespace CustomerService.Api.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("UserId is required.")
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("UserId must be a valid GUID.");
    }
}