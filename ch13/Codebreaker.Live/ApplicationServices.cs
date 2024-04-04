namespace Codebreaker.Live;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSignalR()
            .AddMessagePackProtocol()
            .AddNamedAzureSignalR("signalr");
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
