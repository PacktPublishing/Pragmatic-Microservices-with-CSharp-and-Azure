using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.UI.Components;
public partial class CodeBreakerCard : ComponentBase
{
    [Parameter]
    public RenderFragment HeaderContent { get; set; } = default!;

    [Parameter]
    public RenderFragment HeaderActions { get; set; } = default!;

    [Parameter]
    public RenderFragment CardContent { get; set; } = default!;

    [Parameter]
    public RenderFragment CardActions { get; set; } = default!;

    [Parameter]
    public string Class { get; set; } = string.Empty;
}
