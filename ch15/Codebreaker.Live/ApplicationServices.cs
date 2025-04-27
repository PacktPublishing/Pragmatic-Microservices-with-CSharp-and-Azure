namespace Codebreaker.Live;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var signalRBuilder = builder.Services.AddSignalR()
            .AddMessagePackProtocol();

        if (builder.Configuration["StartupMode"] is not "OnPremises")  // Azure
        {
            signalRBuilder.AddNamedAzureSignalR("signalr");
        }

        builder.AddAzureEventHubConsumerClient("events",
            settings =>
            {
                settings.EventHubName = "completedgames";

                settings.ConsumerGroup = "live";
            });

        builder.Services.AddSingleton<LiveHubClientsCount>();

        // builder.Services.AddGrpc();
    }

    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        // map REST endpoints
        // app.MapLiveGamesEndpoints(logger);
        // map gRPC endpoints
        // app.MapGrpcService<GRPCLiveGameService>();

        // map SignalR hub
        app.MapHub<LiveHub>("/livesubscribe");
        app.MapHub<StreamingLiveHub>("/streaminglivesubscribe");

        return app;
    }
}
