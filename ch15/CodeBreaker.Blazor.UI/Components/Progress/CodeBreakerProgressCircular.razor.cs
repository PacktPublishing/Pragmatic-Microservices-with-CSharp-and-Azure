using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.UI.Components;
public partial class CodeBreakerProgressCircular : ComponentBase
{
    [Parameter]
    public bool Indeterminate { get; set; } = true;

    [Parameter]
    public double Min { get; set; } = default!;

    [Parameter]
    public double Max { get; set; } = default!;

    [Parameter]
    public double Value { get; set; } = default!;

    [Parameter]
    public string Class { get; set; } = string.Empty;
}
