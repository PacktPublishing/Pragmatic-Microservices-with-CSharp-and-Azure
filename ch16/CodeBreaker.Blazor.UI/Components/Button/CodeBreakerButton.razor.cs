using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.UI.Components;
public partial class CodeBreakerButton : ComponentBase
{
    [Parameter]
    public bool Disabled { get; set; } = false;

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public string Class { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }
}
