using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;

namespace CodeBreaker.Blazor.Shared;

public partial class LoginDisplay
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IOptionsSnapshot<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>> Options { get; set; } = default!;

    private void BeginLogOut()
    {
        NavigationManager.NavigateToLogout(Options.Get(Microsoft.Extensions.Options.Options.DefaultName)
            .AuthenticationPaths.LogOutPath);
    }

    private void BeginLogIn()
    {
        NavigationManager.NavigateToLogin(Options.Get(Microsoft.Extensions.Options.Options.DefaultName)
            .AuthenticationPaths.LogInPath);
    }
}
