using CodeBreaker.Blazor.UI.Models.Icon;
using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.UI.Components;

public partial class CodeBreakerIconButton : CodeBreakerButton
{
    private RenderFragment? _icon;

    [Parameter]
    public CodeBreakerIcon Icon { get; set; }

    protected override void OnInitialized()
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
        _icon = builder =>
        {
            builder.OpenElement(0, "i");
            builder.AddAttribute(1, "class", GetClassName(Icon));
            builder.CloseElement();
        };

        base.OnInitialized();
    }
}
