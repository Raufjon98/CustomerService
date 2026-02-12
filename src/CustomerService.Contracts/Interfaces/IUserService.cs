using Customer.Contracts.User.Requests;
using Customer.Contracts.User.Responses;
using FluentResults;
using MagicOnion;

namespace CustomerService.Contracts.Interfaces;

public interface IUserService : IService<IUserService>
{
    UnaryResult<List<UserResponse>> GetUsersAsync();
    UnaryResult<UserResponse?> GetUserAsync(string userId);
    UnaryResult<UserResponse> UpdateUserAsync(UpdateUserRequest request, string userId);
    UnaryResult<bool> DeleteUserAsync(string userId);
    UnaryResult<bool> UpdateUserPassword(string userId, string oldPassword, string newPassword);
}