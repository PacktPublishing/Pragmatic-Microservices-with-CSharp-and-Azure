using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Components;
using CodeBreaker.Blazor.Resources;
using CodeBreaker.Blazor.UI.Models.Icon;
using CodeBreaker.Blazor.UI.Models.Menu;

namespace CodeBreaker.Blazor.Shared;

public partial class MainLayout : LayoutComponentBase
{
    private IEnumerable<NavLinkItem> _menuItems = [];

    [Inject]
    private IStringLocalizer<Resource> Loc { get; init; } = default!;

    protected override void OnInitialized()
    {
        _menuItems = [
            new NavLinkItem(Loc["NavMenu_Home_Title"], "/", NavLinkMatch.All, CodeBreakerIcon.Dashboard),
            new NavLinkItem(@Loc["NavMenu_Game_Title"], "/game", NavLinkMatch.All, CodeBreakerIcon.Game),
            new NavLinkItem(@Loc["NavMenu_Reports_Title"], "/reports", NavLinkMatch.All, CodeBreakerIcon.Reports),
            new NavLinkItem(@Loc["NavMenu_About_Title"], "/about", NavLinkMatch.All, CodeBreakerIcon.About),
        ];
        base.OnInitialized();
    }
}
