using System.Text.RegularExpressions;
using CustomerService.Contracts.Authorization.Requests;
using FluentValidation;

namespace CustomerService.Api.Features.Authentication.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty()
            .EmailAddress();
        
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
        
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters");
        
        RuleFor(x => x.Firstname)
            .NotEmpty()
            .WithMessage("First name is required")
            .MinimumLength(3)
            .WithMessage("Firstname must be at least 3 characters")
            .MaximumLength(50)
            .WithMessage("Firstname must not exceed 50 characters");
        
        RuleFor(x => x.Lastname)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MinimumLength(3)
            .WithMessage("Lastname must be at least 3 characters")
            .MaximumLength(50)
            .WithMessage("Lastname must not exceed 50 characters");
        
        RuleFor(x => x.DateOfBirth)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Date of birth is required")
            .Must(dob => dob <= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-18))
            .WithMessage("Age must be at least 18 years old");
    }
}