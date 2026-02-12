using CustomerService.Api.Features.Authentication;
using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.Authorization.Responses;
using CustomerService.Contracts.Interfaces;
using MagicOnion;
using MagicOnion.Server;
using MediatR;

namespace CustomerService.Api.MagicOnion.Services;

public class AuthService : ServiceBase<IAuthService>,  IAuthService
{
    private readonly IMediator _mediator;

    public AuthService(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async UnaryResult<LoginResponse> LoginAsync(LoginRequest request)
    {
       var command = new LoginCommand(request);
       var result = await _mediator.Send(command);
       return result;
    }

    public async UnaryResult<bool> RegisterAsync(RegisterRequest request)
    {
        var command = new RegisterCommand(request);
        var result = await _mediator.Send(command);
        return result;
    }

    public async UnaryResult<bool> LogoutAsync()
    {
        await _mediator.Send(new LogoutCommand());
        return true;
    }
}