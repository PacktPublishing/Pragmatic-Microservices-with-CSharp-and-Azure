using CodeBreaker.Blazor.UI.Models.Icon;
using CodeBreaker.Blazor.UI.Models.Menu;
using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.UI.Components;
public partial class CodeBreakerNavMenu
{
    [Parameter, EditorRequired]
    public IEnumerable<NavLinkItem> MenuItems { get; set; } = Enumerable.Empty<NavLinkItem>();

    private static RenderFragment GetIcon(CodeBreakerIcon icon)
    {
        static string? GetClassName(CodeBreakerIcon icon) => icon switch
        {
            CodeBreakerIcon.Cancel => "fa-solid fa-xmark",
            CodeBreakerIcon.Play => "fa-solid fa-play",
            CodeBreakerIcon.Global => "fa-solid fa-language",
            CodeBreakerIcon.Login => "fa-solid fa-arrow-right-to-bracket",
            CodeBreakerIcon.Logout => "fa-solid fa-arrow-right-from-bracket",
            CodeBreakerIcon.Dashboard => "fa-solid fa-house",
            CodeBreakerIcon.Game => "fa-solid fa-chess-board",
            CodeBreakerIcon.Reports => "fa-solid fa-tachograph-digital",
            CodeBreakerIcon.About => "fa-solid fa-address-card",
            _ => null
        };
        return builder =>
        {
            builder.OpenElement(0, "i");
            builder.AddAttribute(1, "class", GetClassName(icon));
            builder.CloseElement();
        };
    }
}