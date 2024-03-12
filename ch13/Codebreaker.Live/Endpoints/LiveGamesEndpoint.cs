namespace Codebreaker.Live.Endpoints;

public static class LiveGamesEndpoint
{
    public static void MapLiveGamesEndpoint(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/live")
            .WithTags("Game Events API");

        group.MapPost("/game", async (GameSummary gameSummary, IHubContext<LiveHub> hubContext) =>
        {
            await hubContext.Clients.Group(gameSummary.GameType).SendAsync("GameCompleted", gameSummary);
            return TypedResults.Ok();
        });
    }
}
