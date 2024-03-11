using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.Client.Pages;

public partial class IndexPage
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private void NavigateTo(string url)
    {
        NavigationManager.NavigateTo(url);
    }
}
