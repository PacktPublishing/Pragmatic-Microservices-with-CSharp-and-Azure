namespace Codebreaker.Live;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var signalRBuilder = builder.Services.AddSignalR()
            .AddMessagePackProtocol();

        if (Environment.GetEnvironmentVariable("StartupMode") != "OnPremises")
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
