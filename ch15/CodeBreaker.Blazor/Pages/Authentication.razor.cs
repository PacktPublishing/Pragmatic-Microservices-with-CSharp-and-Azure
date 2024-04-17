using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.Pages;

public partial class Authentication
{
    [Inject]
    private NavigationManager NavigationManager { get; init; } = default!;

    [Parameter]
    public string Action { get; set; } = string.Empty;

    private void OnLogout()
    {
        NavigationManager.NavigateTo("/", true, true);
    }
}
