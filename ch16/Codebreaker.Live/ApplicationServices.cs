using Azure.Identity;

namespace Codebreaker.Live;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var signalRBuilder = builder.Services.AddSignalR()
            .AddMessagePackProtocol();

        if (builder.Configuration["StartupMode"] is not "OnPremises")
        {
            signalRBuilder.AddNamedAzureSignalR("signalr");
        }

        builder.AddAzureEventHubConsumerClient("codebreakerevents",
            settings =>
            {
                // settings.Credential = new AzureCliCredential();
                settings.EventHubName = "games";
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
