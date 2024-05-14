using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;

namespace CodeBreaker.Blazor.Client.Shared;

public partial class RedirectToLogin
{
    [Inject]
    private IOptionsSnapshot<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>> Options { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;

    protected override void OnInitialized()
    {
        NavigationManager.NavigateToLogin("authentication/login");
    }
}
