using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CodeBreaker.Blazor.UI.Components;

public partial class CodeBreakerThemeSwitch : ComponentBase
{
    private bool _isInitialized = false;
    private IJSObjectReference? _module;
    private bool IsDark = false;

    //[Inject]
    //protected IThemeService<T> ThemeService { get; set; } = default!;

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    protected async Task SwitchTheme(bool isDark)
    {
        //IsDark = isDark;
        //await _themeService.SwitchThemeAsync(IsDark);
    }

    protected override async Task OnInitializedAsync()
    {
        //var context = await ThemeService.GetCurrentThemeAsync();
        IsDark = false;// context.IsDark;
        await base.OnInitializedAsync();
        _isInitialized = true;
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _module == null)
            _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/CodeBreaker.Blazor.UI/Components/ThemeSwitch/CodeBreakerThemeSwitch.razor.js");

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnSwitchTheme()
    {
        IsDark = !IsDark;
        //await ThemeService.SwitchThemeAsync(IsDark);

        if (_module != null)
            await _module.InvokeVoidAsync("switchTheme", IsDark);
    }
}
