using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Codebreaker.Client;
public class AuthenticationService
{
    private string[] _scopes; // =
    //[
    //    "https://codebreaker3000.onmicrosoft.com/39a665a4-54ce-44cd-b581-0f654a31dbcf/Games.Play",
    //    "https://codebreaker3000.onmicrosoft.com/39a665a4-54ce-44cd-b581-0f654a31dbcf/Reports.Readd"
    //    //"user.read",
    //    // "api://383bf030-0772-4c0d-b49a-27009dc9504/access_as_user"
    //];

    private readonly IPublicClientApplication _publicClientApp;

    public AuthenticationService(IOptions<AuthenticationServiceOptions> options)
    {
        string clientId = options.Value?.ClientId ?? throw new InvalidOperationException("Could not read ClientId");
        string redirectUri = options.Value?.RedirectUri ?? throw new InvalidOperationException("Could not read RedirectUri");
        string authorityUri = options.Value?.AuthorityUri ?? throw new InvalidOperationException("Could not read AuthorityUri");
        _scopes = options.Value?.Scopes ?? throw new InvalidOperationException("Could not read Scopes");

        _publicClientApp = PublicClientApplicationBuilder.Create(clientId)
            .WithAuthority(authorityUri)
//            .WithB2CAuthority(authorityUri)
            .WithRedirectUri(redirectUri)
            .Build();
    }

    public async Task<AuthenticationResult> LoginAsync()
    {
        async Task<AuthenticationResult> GetTokenInteractiveAsync()
        {
            AuthenticationResult result = await _publicClientApp.AcquireTokenInteractive(_scopes)
                .ExecuteAsync();
            return result;
        }

        try
        {
            IEnumerable<IAccount> accounts = await _publicClientApp.GetAccountsAsync();
            if (accounts == null || !accounts.Any())
            {
                return await GetTokenInteractiveAsync();
            }
            // TODO: if this fails, use GetTokenInteractiveAsync as well!
            AuthenticationResult result = await _publicClientApp.AcquireTokenSilent(_scopes, accounts.First())
                .ExecuteAsync();
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}

public class AuthenticationServiceOptions
{
    public string? ClientId { get; set; }
    public string? RedirectUri { get; set; }
    public string? AuthorityUri { get; set; }
    public string[]? Scopes { get; set; }
}
