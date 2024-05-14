using System.Reflection;
using CodeBreaker.Blazor.Client.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace CodeBreaker.Blazor.Client.Pages;

public partial class AboutPage
{
    [Inject]
    private IStringLocalizer<Resource> Loc { get; set; } = default!;

    private string instructions = string.Empty;
    private string version = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        instructions = Loc["About_Instructions"];
        var currentVersion = typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        version = string.IsNullOrWhiteSpace(currentVersion) ? Loc["About_NoVersion_Found"] : currentVersion;
        await base.OnInitializedAsync();
    }
}
