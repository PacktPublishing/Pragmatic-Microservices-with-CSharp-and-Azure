namespace Codebreaker.Live;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSignalR()
            .AddMessagePackProtocol()
            .AddNamedAzureSignalR("signalr");

        builder.Services.AddGrpc();
    }

    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        // map REST endpoints
        // app.MapLiveGamesEndpoints(logger);
        // map gRPC endpoints
        app.MapGrpcService<GRPCLiveGameService>();

        // map SignalR hub
        app.MapHub<LiveHub>("/livesubscribe");
        return app;
    }
}
