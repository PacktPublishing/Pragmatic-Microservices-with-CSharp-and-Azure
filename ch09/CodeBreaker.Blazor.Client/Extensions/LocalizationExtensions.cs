using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

namespace CodeBreaker.Blazor.Client.Extensions;

internal static class LocalizationExtensions
{
    public static async Task ConfigureLocalizationAsync(this WebAssemblyHost host)
    {
        CultureInfo culture;
        var js = host.Services.GetRequiredService<IJSRuntime>();
        var result = await js.InvokeAsync<string>("blazorCulture.get");

        if (result != null)
        {
            culture = new(result);
        }
        else
        {
            culture = new("en");
            await js.InvokeVoidAsync("blazorCulture.set", "en");
        }

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
