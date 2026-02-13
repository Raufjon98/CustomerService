using Customer.Contracts.User.Requests;
using Customer.Contracts.User.Responses;
using CustomerService.Api.Domain;
using CustomerService.Api.Features.Common.Exceptions;
using CustomerService.Api.Infrastructure.Data;
using CustomerService.Contracts.User.Events;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Api.Features.Users.Commands;

public record UpdateUserCommand(string UserId, UpdateUserRequest UpdateUserRequest) : IRequest<UserResponse>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateUserCommandHandler(ApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }
    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userFromDb = await _context.Users
            .FirstOrDefaultAsync(u=> u.Id == request.UserId, cancellationToken);

        if (userFromDb is null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }
        
        userFromDb.UserName = request.UpdateUserRequest.Username;
        userFromDb.FirstName = request.UpdateUserRequest.FirstName;
        userFromDb.LastName = request.UpdateUserRequest.LastName;
        userFromDb.MiddleName = request.UpdateUserRequest.MiddleName;
        userFromDb.Email = request.UpdateUserRequest.Email;
        userFromDb.PhoneNumber = request.UpdateUserRequest.Phone;
        _context.Users.Update(userFromDb) ;
        await _context.SaveChangesAsync(cancellationToken);
        await _publishEndpoint.Publish(
            new UserUpdatedEvent()
            {
                Id= userFromDb.Id,
                UpdatedOnUtc = DateTime.UtcNow
            },
            cancellationToken);
     
        return new UserResponse()
        {
            Id = userFromDb.Id,
            UserName = userFromDb.UserName,
            FirstName = userFromDb.FirstName,
            LastName = userFromDb.LastName,
            Age = DateTime.Today.Year - userFromDb.DateOfBirth.Year,
        };
    }
}