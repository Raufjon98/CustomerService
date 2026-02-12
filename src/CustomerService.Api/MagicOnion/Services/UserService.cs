using Customer.Contracts.User.Requests;
using Customer.Contracts.User.Responses;
using CustomerService.Api.Features.Users.Commands;
using CustomerService.Api.Features.Users.Queries;
using CustomerService.Contracts.Interfaces;
using MagicOnion;
using MagicOnion.Server;
using MediatR;

namespace CustomerService.Api.MagicOnion.Services;

public class UserService : ServiceBase<IUserService>, IUserService
{
    private readonly IMediator _mediator;

    public UserService(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async UnaryResult<List<UserResponse>> GetUsersAsync()
    {
        var query = new GetUsersQuery();
        var result = await _mediator.Send(query);
        return result;
    }

    public async UnaryResult<UserResponse?> GetUserAsync(string userId)
    {
        var query = new GetUserQuery(userId);
        var result = await _mediator.Send(query);
        return result;
    }

    public async UnaryResult<UserResponse> UpdateUserAsync(UpdateUserRequest request, string userId)
    {
        var command = new UpdateUserCommand(userId, request);
        var result = await _mediator.Send(command);
        return result;
    }

    public async UnaryResult<bool> DeleteUserAsync(string userId)
    {
        var command = new DeleteUserCommand(userId);
        var result = await _mediator.Send(command);
        return result;
    }

    public async UnaryResult<bool> UpdateUserPassword(string userId, string oldPassword, string newPassword)
    {
        var command = new UpdateUserPasswordCommand(userId, oldPassword, newPassword);
        var result = await _mediator.Send(command);
        return result;
    }
}