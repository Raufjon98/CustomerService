using Customer.Contracts.User.Responses;
using CustomerService.Api.Domain;
using CustomerService.Api.Features.Common.Exceptions;
using CustomerService.Api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Api.Features.Users.Queries;

public record GetUserQuery(string UserId) : IRequest<UserResponse?>;
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserResponse?>
{
    private readonly ApplicationDbContext _context;

    public GetUserQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<UserResponse?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(u => u.Id == request.UserId)
            .Select(u=> new UserResponse()
            {
                Id = u.Id,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Age = DateTime.Today.Year -  u.DateOfBirth.Year,
            }).FirstOrDefaultAsync(cancellationToken);
        
        if (user is null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }
        
        return user;
    }
}