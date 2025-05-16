using Codebreaker.ServiceDefaults;

namespace Codebreaker.Live;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var signalRBuilder = builder.Services.AddSignalR()
            .AddMessagePackProtocol();

        if (Environment.GetEnvironmentVariable(EnvVarNames.LiveGameMonitoring) == LiveGameMonitoringType.SignalRWithAzure.ToString())
        { 
            signalRBuilder.AddNamedAzureSignalR("signalr");
        }
    }

    public static WebApplication MapApplicationEndpoints(this WebApplication app, ILogger logger)
    {
        // map REST endpoints
        app.MapLiveGamesEndpoints(logger);

        // map SignalR hub
        app.MapHub<LiveHub>("/livesubscribe");
        return app;
    }
}
