using Codebreaker.GameAPIs.Client;
using CodeBreaker.Blazor.UI.Services.Dialog;

namespace CodeBreaker.Blazor;

internal static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IGamesClient, GamesClient>(client =>
        {
            client.BaseAddress = new Uri("http://gameapis");
        });

        builder.Services.AddScoped<IDialogService, DialogService>();
    }
}
