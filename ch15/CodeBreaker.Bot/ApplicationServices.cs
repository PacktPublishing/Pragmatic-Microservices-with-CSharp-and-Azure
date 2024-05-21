using Codebreaker.Grpc;

namespace CodeBreaker.Bot;

internal static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // when using gRPC to communicate with the game API, uncomment this section, and comment the AddHttpClient section
        //builder.Services.AddSingleton<IGamesClient, GrpcGamesClient>()
        //    .AddGrpcClient<GrpcGame.GrpcGameClient>(
        //        client =>
        //        {
        //            client.Address = new Uri("https://gameapis");
        //        });

        builder.Services.AddHttpClient<IGamesClient, GamesClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://gameapis");
        });

        builder.Services.AddScoped<CodeBreakerTimer>();
        builder.Services.AddScoped<CodeBreakerGameRunner>();
    }
}
