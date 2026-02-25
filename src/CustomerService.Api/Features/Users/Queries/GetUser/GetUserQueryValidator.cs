using FluentValidation;

namespace CustomerService.Api.Features.Users.Queries.GetUser;

public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("UserId is required.")
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("UserId must be a valid GUID.");
    }   
}