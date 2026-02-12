using Customer.Contracts.User.Responses;
using CustomerService.Api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Api.Features.Users.Queries;

public record GetUsersQuery : IRequest<List<UserResponse>>;
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserResponse>>
{
    private readonly ApplicationDbContext _context;

    public GetUsersQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _context.Users.Where(u=>u.IsDelete == false).ToListAsync(cancellationToken);
        List<UserResponse> result = new List<UserResponse>();
        foreach (var u in users)
        {
            UserResponse response = new UserResponse()
            {
                Id = u.Id,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Age = DateTime.Today.Year -  u.DateOfBirth.Year,
            };
            result.Add(response);
        }
        return result;
    }
}