
using Codebreaker.BotQ.Endpoints;
using Codebreaker.Grpc;

namespace CodeBreaker.Bot;

internal static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddAzureQueueClient("botqueue");
        builder.Services.AddScoped<BotQueueClient>();
        var section = builder.Configuration.GetSection("Bot");
        builder.Services.Configure<BotQueueClientOptions>(section);
        builder.Services.AddScoped<CodebreakerTimer>();
        builder.Services.AddScoped<CodebreakerGameRunner>();

        builder.Services.AddSingleton<IGamesClient, GrpcGamesClient>()
            .AddGrpcClient<GrpcGame.GrpcGameClient>(
        client =>
        {
            var endpoint = builder.Configuration["services:gameapis:https:0"] ?? throw new InvalidOperationException();
            client.Address = new Uri(endpoint);

            // TODO: change to with a later version:
            // client.Address = new Uri("https://gameapis");
        });
    }
}
