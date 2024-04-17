using Codebreaker.Grpc;

namespace CodeBreaker.Bot;

internal static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        //builder.Services.AddSingleton<IGamesClient, GrpcGamesClient>()
        //    .AddGrpcClient<GrpcGame.GrpcGameClient>(
        //        client =>
        //        {
        //            //var resolver = sp.GetRequiredService<ServiceEndpointResolver>();

        //            //var endpointSource = await resolver.GetEndpointsAsync("https+http://gameapis", CancellationToken.None);

        //            //var endpoint = endpointSource.Endpoints
        //            //    .Select(e => e.EndPoint).First();

        //            //client.Address = new Uri(endpoint.ToString() ?? throw new InvalidOperationException());

        //            var endpoint = builder.Configuration["services:gameapis:https:0"] ?? throw new InvalidOperationException();
        //            client.Address = new Uri(endpoint);

        //            // TODO: change to with a later version:
        //            // client.Address = new Uri("https://gameapis");
        //        });

        builder.Services.AddHttpClient<IGamesClient, GamesClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://gameapis");
        });

        builder.Services.AddScoped<CodeBreakerTimer>();
        builder.Services.AddScoped<CodeBreakerGameRunner>();
    }
}
