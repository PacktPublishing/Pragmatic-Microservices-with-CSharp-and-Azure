using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.UI.Components;
public abstract class CodeBreakerInputBase : ComponentBase
{
    [Parameter]
    public string Value { get; set; } = string.Empty;
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }
    [Parameter]
    public bool Required { get; set; } = false;
    [Parameter]
    public string RequiredError { get; set; } = string.Empty;
    [Parameter]
    public string Label { get; set; } = string.Empty;
    [Parameter]
    public string Placeholder { get; set; } = string.Empty;
    [Parameter]
    public string CssClass { get; set; } = string.Empty;
    [Parameter]
    public int MaxLength { get; set; } = int.MaxValue;
}
