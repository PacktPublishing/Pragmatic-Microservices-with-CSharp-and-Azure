using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace CodeBreaker.Blazor.UI.Components;
public partial class CodeBreakerSelect<T> : ComponentBase
{
    private string _selectedKey = string.Empty;
    private bool _isOpen;

    [Parameter, EditorRequired]
    public Dictionary<string, T> Items { get; set; } = default!;

    [Parameter]
    public string Label { get; set; } = string.Empty;

    [Parameter]
    public T Value { get; set; } = default!;

    [Parameter]
    public EventCallback<T> ValueChanged { get; set; } = default!;

    [Parameter]
    public Expression<Func<T>> ValueExpression { get; set; } = default!;

    protected override void OnInitialized()
    {
        var selectedItem = Items.FirstOrDefault(i => i.Value?.GetHashCode() == Value?.GetHashCode()).Key;

        if (!string.IsNullOrWhiteSpace(selectedItem))
            _selectedKey = selectedItem;

        base.OnInitialized();
    }

    private async Task OnSelectionChanged(string key, T value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(Value);
        _selectedKey = key;
        _isOpen = false;
    }
}
