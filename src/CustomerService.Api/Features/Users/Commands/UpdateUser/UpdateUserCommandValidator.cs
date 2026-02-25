using CustomerService.Api.Features.Users.Validators;
using FluentValidation;

namespace CustomerService.Api.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("UserId is required.")
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("UserId must be a valid GUID.");
        
        RuleFor(x => x.UpdateUserRequest)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Update User is required.")
            .SetValidator(new UpdateUserRequestValidator());
    }
}