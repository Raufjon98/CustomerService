using System.Text.RegularExpressions;
using CustomerService.Contracts.Authorization.Requests;
using FluentValidation;

namespace CustomerService.Api.Features.Authentication.Validators;

public class LoginRequestValidator :AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters");
        
        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters")
            .MaximumLength(50)
            .WithMessage("Password must not exceed 50 characters")
            .Must(p => char.IsUpper(p[0]) && 
                                Regex.IsMatch(p, @"[\W_]"))
            .WithMessage("Password must start with uppercase and have at least one symbol");
    }
}