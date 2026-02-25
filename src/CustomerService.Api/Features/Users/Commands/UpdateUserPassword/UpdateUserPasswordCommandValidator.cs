using System.Text.RegularExpressions;
using FluentValidation;

namespace CustomerService.Api.Features.Users.Commands.UpdateUserPassword;

public class UpdateUserPasswordCommandValidator :  AbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("UserId is required.")
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("UserId must be a valid GUID.");
        
        RuleFor(x => x.NewPassword)
            .MinimumLength(8)
            .MaximumLength(50)
            .Must(p => char.IsUpper(p[0]) && 
                       Regex.IsMatch(p, @"[\W_]"))
            .WithMessage("New password must start with uppercase and have at least one symbol");
        
        RuleFor(x => x.OldPassword)
            .MinimumLength(8)
            .MaximumLength(50)
            .Must(p => char.IsUpper(p[0]) && 
                       Regex.IsMatch(p, @"[\W_]"))
            .WithMessage("Old password must start with uppercase and have at least one symbol");
    }
}