using CustomerService.Api.Features.Authentication.Validators;
using FluentValidation;

namespace CustomerService.Api.Features.Authentication.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Register)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Please specify a register")
            .SetValidator(new RegisterRequestValidator());
    }
}