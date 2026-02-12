using CustomerService.Api.Domain;

namespace CustomerService.Api.Interfaces;

public interface ICurrentUserService
{
    //CreateRespinse
    Task<User> GetCurrentUser();
}