using Codebreaker.Client;
using Codebreaker.ViewModels.Contracts.Services;
using Codebreaker.WPF.Services;

namespace Codebreaker.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App : Application, IDisposable
{
    private readonly IHost _host;

    public App()
    {
#if DEBUG
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
#else
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Production");
#endif

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(options =>
            {
                string? test = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            })
            .ConfigureServices((context, services) =>
            {
                services.AddTransient<IDialogService, Services.WPFDialogService>();
                services.AddScoped<GamePageViewModel>();
                services.AddScoped<IInfoBarService, InfoBarService>();
                services.AddHttpClient<IGamesClient, GamesClient>(client =>
                {
                    string uriString = context.Configuration["ApiBase"] ?? throw new ConfigurationErrorsException("ApiBase not configured");
                    client.BaseAddress = new Uri(uriString);
                });
                services.AddSingleton<AuthenticationService>();
                services.Configure<AuthenticationServiceOptions>(context.Configuration.GetSection("Authentication"));
            })
            .Build();

        DefaultScope = _host.Services.CreateScope();
    }

    public void Dispose()
    {
        DefaultScope.Dispose();
        _host.Dispose();
    }

    public IServiceScope DefaultScope { get; private init; }

    public IServiceProvider Services => _host.Services;

    public T GetService<T>()
        where T : class =>
        DefaultScope.ServiceProvider.GetRequiredService<T>();

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
    }
}
