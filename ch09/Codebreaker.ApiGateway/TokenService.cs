using System.Security.Claims;

namespace Codebreaker.Proxy;

internal class TokenService
{
    internal Task<string?> GetAuthTokenAsync(ClaimsPrincipal user)
    {
        // TODO: use authentication API
        if (user.Identity?.Name == "Bob")
        {
            return Task.FromResult<string?>(Guid.NewGuid().ToString());
        }
        return Task.FromResult<string?>(null);
    }
}
