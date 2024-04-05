using CodeBreaker.Blazor.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;

namespace CodeBreaker.Blazor.Components;

public partial class LanguageSelector
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = default!;

    [Inject]
    private IStringLocalizer<Resource> Loc { get; set; } = default!;

    private readonly Dictionary<string, CultureInfo> _items = [];

    private CultureInfo Culture
    {
        get => CultureInfo.CurrentCulture;
        set
        {
            if (CultureInfo.CurrentCulture != value)
            {
                var js = (IJSInProcessRuntime)JsRuntime;
                js.InvokeVoid("blazorCulture.set", value.Name);
                NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
            }
        }
    }

    protected override void OnInitialized()
    {
        _items.Add(Loc["Language_English"], new CultureInfo("en"));
        _items.Add(Loc["Language_German"], new CultureInfo("de"));
        base.OnInitialized();
    }
}
