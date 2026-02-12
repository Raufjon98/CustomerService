using CustomerService.Api.Domain;
using CustomerService.Api.Features.Common.Exceptions;
using CustomerService.Api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.Contracts.Interfaces;

namespace CustomerService.Api.Features.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest<bool>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly IAccountService  _accountService; 

    public DeleteUserCommandHandler(ApplicationDbContext context,
        IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(u => u.Id == request.UserId && u.IsDelete == false)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }
        user.IsDelete = true;
        user.Email = $"deleted_{user.Email}_{Guid.NewGuid()}";
        user.UserName = $"deleted_{user.UserName}_{Guid.NewGuid()}";

        user.NormalizedEmail = user.Email.ToUpper();
        user.NormalizedUserName = user.UserName.ToUpper();
        
        if (!Guid.TryParse(user.Id, out  var customerId))
        {
            throw new InvalidDataException("Invalid customerId");
        }
        
        var accoount = await _accountService.DeleteAccountAsync(customerId);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}