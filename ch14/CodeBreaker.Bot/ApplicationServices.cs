using Codebreaker.Grpc;

using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.ServiceDiscovery;
// using Microsoft.Extensions.ServiceDiscovery;

namespace CodeBreaker.Bot;

internal static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // services__gameapis__https__0

        builder.Services.AddSingleton<IGamesClient, GrpcGamesClient>()
            .AddGrpcClient<GrpcGame.GrpcGameClient>(
                async (sp, client) =>
                {
                    //var resolver = sp.GetRequiredService<ServiceEndpointResolver>();

                    //var endpointSource = await resolver.GetEndpointsAsync("https+http://gameapis", CancellationToken.None);

                    //var endpoint = endpointSource.Endpoints
                    //    .Select(e => e.EndPoint).First();

                    //client.Address = new Uri(endpoint.ToString() ?? throw new InvalidOperationException());

                    var endpoint = builder.Configuration["services:gameapis:https:0"] ?? throw new InvalidOperationException();
                    client.Address = new Uri(endpoint);

                    // TODO: change to with a later version:
                    // client.Address = new Uri("https://gameapis");
                });

        builder.Services.AddScoped<CodeBreakerTimer>();
        builder.Services.AddScoped<CodeBreakerGameRunner>();

    }

}
