using System.Linq.Expressions;

namespace CodeBreaker.Blazor.UI.Models.DataGrid;
public class CodeBreakerColumnDefinition<T>(
    string fieldName,
    Expression<Func<T, object>> fieldSelectorExpression,
    bool showMobile = true,
    string? value = null)
{
    public string ColumnDataKey { get; set; } = fieldName;

    public string Title { get; set; } = fieldName;

    public Func<T, object>? FieldSelector { get; set; } = fieldSelectorExpression.Compile();

    public Expression<Func<T, object>>? FieldSelectorExpression { get; set; } = fieldSelectorExpression;

    public string? Value { get; set; } = value;

    public bool ShowMobile { get; set; } = showMobile;
}
