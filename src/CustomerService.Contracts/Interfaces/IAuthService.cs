using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.Authorization.Responses;
using MagicOnion;

namespace CustomerService.Contracts.Interfaces;

public interface IAuthService : IService<IAuthService>
{
    UnaryResult<LoginResponse> LoginAsync(LoginRequest request);   
    UnaryResult<bool> RegisterAsync(RegisterRequest request);
    UnaryResult<bool> LogoutAsync();
}