using CodeBreaker.Blazor.UI.Models.DataGrid;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace CodeBreaker.Blazor.UI.Components;

public partial class CodeBreakerDataGrid<T> : ComponentBase
{
    private IQueryable<T>? _source;
    private readonly PaginationState pagination = new() { ItemsPerPage = 7 };

    [Parameter]
    public List<string> Headers { get; set; } = [];

    [Parameter]
    public List<T> Items { get; set; } = [];

    [Parameter]
    public List<CodeBreakerColumnDefinition<T>> Columns { get; set; } = [];

    [Parameter]
    public EventCallback<T> RowItemClicked { get; set; } = new();

    protected override void OnInitialized()
    {
        _source = Items.AsQueryable();
        pagination.TotalItemCountChanged += (sender, eventArgs) => StateHasChanged();
        base.OnInitialized();
    }
    private async Task GoToPageAsync(int pageIndex)
    {
        await pagination.SetCurrentPageIndexAsync(pageIndex);
    }

    private string? PageButtonClass(int pageIndex)
        => pagination.CurrentPageIndex == pageIndex ? "current" : null;

    private string? AriaCurrentValue(int pageIndex)
        => pagination.CurrentPageIndex == pageIndex ? "page" : null;

    private async Task Hire(T context) 
    {
        await RowItemClicked.InvokeAsync(context);
    }
}
