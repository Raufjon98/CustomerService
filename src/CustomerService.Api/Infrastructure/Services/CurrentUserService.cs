using System.Security.Claims;
using CustomerService.Api.Domain;
using CustomerService.Api.Interfaces;

namespace CustomerService.Api.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public  Task<User> GetCurrentUser()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            throw new InvalidOperationException("HttpContext must not be null");
        }

        var userId =  _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException("UserId is empty");
        }
        
        var username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.GivenName)?.Value;
        if (string.IsNullOrEmpty(username))
        {
            throw new InvalidOperationException("Username is empty");
        }
        
        var email  =  _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            throw new InvalidOperationException("Email is empty");
        }
        
        User user = new User()
        {
            Id = userId,
            Email = email,
            UserName = username,
        };
        return Task.FromResult(user);
    }
}