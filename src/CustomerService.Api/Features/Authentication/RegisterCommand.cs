using CustomerService.Api.Domain;
using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.User.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Api.Features.Authentication;

public record RegisterCommand(RegisterRequest Register) : IRequest<bool>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
{
    private readonly UserManager<User> _userManager;
    private readonly IPublishEndpoint _publishEndpoint;

    public RegisterCommandHandler(UserManager<User> userManager, IPublishEndpoint publishEndpoint)
    {
        _userManager = userManager;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        User user = new User
        {
            Email = request.Register.Email,
            UserName = request.Register.Username,
            LastName = request.Register.Lastname,
            FirstName = request.Register.Firstname,
            DateOfBirth = request.Register.DateOfBirth
        };

        if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == user.Email.ToUpper()
                                                   && u.NormalizedUserName == user.UserName.ToUpper()
                                                   && u.IsDelete == true, cancellationToken))
        {
            return false;
        }

        var result = await _userManager.CreateAsync(user, request.Register.Password);

        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
            if (roleResult.Succeeded)
            {
                await _publishEndpoint.Publish(
                    new RegisteredEvent
                    {
                        Id = user.Id,
                        Email = user.Email,
                        RegisteredOnUtc = DateTime.UtcNow
                    },
                    cancellationToken);
                
                return true;
            }
        }

        return false;
    }
}