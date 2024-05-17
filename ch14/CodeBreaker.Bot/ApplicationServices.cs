using Codebreaker.Grpc;

using Microsoft.Extensions.ServiceDiscovery;

namespace CodeBreaker.Bot;

internal static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IGamesClient, GrpcGamesClient>()
            .AddGrpcClient<GrpcGame.GrpcGameClient>(
            (sp, client) =>
        {
            // https+http:// is not resolved with 8.0; it's necessary to decide one:
            client.Address = new Uri("https://gameapis");
        });

        builder.Services.AddScoped<CodeBreakerTimer>();
        builder.Services.AddScoped<CodeBreakerGameRunner>();
    }
}
