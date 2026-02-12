using CustomerService.Api.Domain;

namespace CustomerService.Api.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(User user);
}