namespace Codebreaker.Live;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSignalR()
            .AddMessagePackProtocol()
            .AddNamedAzureSignalR("signalr");
    }

    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        // map REST endpoints
        // app.MapLiveGamesEndpoints(logger);
        // map gRPC endpoints
        app.MapGrpcService<LiveGameService>();

        // map SignalR hub
        app.MapHub<LiveHub>("/livesubscribe");
        return app;
    }
}
