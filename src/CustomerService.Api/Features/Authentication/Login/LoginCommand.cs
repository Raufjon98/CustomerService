using CustomerService.Api.Domain;
using CustomerService.Api.Interfaces;
using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.Authorization.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Api.Features.Authentication.Login;

public record LoginCommand(LoginRequest Login) : IRequest<LoginResponse>;
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService)
    {
       _userManager = userManager;
       _signInManager = signInManager;
       _tokenService = tokenService;
    }
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedUserName = request.Login.Username.ToUpper(); 
        var user = await _userManager.Users
            .Where(u=>u.NormalizedUserName == normalizedUserName)
            .FirstOrDefaultAsync( cancellationToken);
        if (user == null)
        {
            throw new Exception("Wrong username or password");
        }
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Login.Password, false);
        if (!signInResult.Succeeded)
        {
           throw new Exception("Wrong username or password");
        }

        return new LoginResponse
        {
            Token = await _tokenService.CreateTokenAsync(user)
        };
    }
}