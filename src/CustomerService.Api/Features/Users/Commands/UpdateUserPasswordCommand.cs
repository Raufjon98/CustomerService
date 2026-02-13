using CustomerService.Api.Domain;
using CustomerService.Api.Features.Common.Exceptions;
using CustomerService.Contracts.User.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Api.Features.Users.Commands;

public record UpdateUserPasswordCommand(string UserId, string OldPassword , string NewPassword) :   IRequest<bool>;
public class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand, bool>
{
    private readonly UserManager<User> _userManager;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateUserPasswordCommandHandler(UserManager<User> userManager, IPublishEndpoint publishEndpoint)
    {
        _userManager = userManager;
        _publishEndpoint = publishEndpoint;
    }
    public async Task<bool> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }
        
        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        await _publishEndpoint.Publish(
            new UserUpdatedEvent()
            {
                Id = user.Id,
                UpdatedOnUtc = DateTime.UtcNow
            }, 
            cancellationToken);
        return result.Succeeded;
    }
}