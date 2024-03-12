using Codebreaker.Live.Endpoints;

namespace Codebreaker.Live;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IGameSummaryService, GameSummaryService>();
    }

    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.MapHub<LiveHub>("/live");
        return app;
    }
}
