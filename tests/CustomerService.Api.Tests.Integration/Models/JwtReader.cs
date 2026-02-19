using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CustomerService.Api.Tests.Integration.Models;

public static class JwtReader
{
    public static Guid GetUserId(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var value = jwt.Claims
            .FirstOrDefault(x =>
                x.Type == ClaimTypes.NameIdentifier ||
                x.Type == JwtRegisteredClaimNames.Sub ||
                x.Type == "nameid")
            ?.Value;

        return value is null ? Guid.Empty : Guid.Parse(value);
    }
}