using CustomerService.Api.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CustomerService.Api.Features.Authentication.Logout;

public record LogoutCommand : IRequest;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly SignInManager<User> _signInManager;

    public LogoutCommandHandler(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
        return Unit.Value;
    }
}