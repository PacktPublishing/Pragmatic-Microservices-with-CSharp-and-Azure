using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.UI.Components;

public partial class CodeBreakerSwitch : ComponentBase
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public bool Value { get; set; }
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }

    private async Task Toggle()
    {
        Value = !Value;
        await ValueChanged.InvokeAsync(Value);
    }
}
