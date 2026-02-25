using CustomerService.Api.Features.Authentication.Validators;
using FluentValidation;

namespace CustomerService.Api.Features.Authentication.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new LoginRequestValidator());
    }
}