using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace CodeBreaker.Blazor.UI.Components;
public partial class CodeBreakerRadioGroup<T> : ComponentBase
{
    [Parameter, EditorRequired]
    public T Value { get; set; } = default!;

    [Parameter]
    public EventCallback<T> ValueChanged { get; set; }

    [Parameter]
    public Expression<Func<T>>? ValueExpression { get; set; }

    [Parameter, EditorRequired]
    public IEnumerable<KeyValuePair<string, T>> Items { get; set; } = default!;
}
