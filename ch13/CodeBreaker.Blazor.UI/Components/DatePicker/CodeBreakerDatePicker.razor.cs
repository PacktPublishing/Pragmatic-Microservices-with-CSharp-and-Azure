using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.UI.Components;
public partial class CodeBreakerDatePicker : ComponentBase
{
    [Parameter]
    public string Label { get; set; } = string.Empty;

    [Parameter]
    public DateOnly? Date { get; set; }

    [Parameter]
    public EventCallback<DateOnly?> DateChanged { get; set; }
}
