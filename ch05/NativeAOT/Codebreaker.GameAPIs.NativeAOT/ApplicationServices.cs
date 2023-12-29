using Codebreaker.GameAPIs.Data.InMemory;

namespace Codebreaker.GameAPIs;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
        builder.Services.AddScoped<IGamesService, GamesService>();
    }
}
