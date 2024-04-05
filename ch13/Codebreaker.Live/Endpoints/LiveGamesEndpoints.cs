namespace Codebreaker.Live.Endpoints;

public static class LiveGamesEndpoints
{
    public static void MapLiveGamesEndpoints(this IEndpointRouteBuilder routes, ILogger logger)
    {
        var group = routes.MapGroup("/live")
            .WithTags("Game Events API");

        group.MapPost("/game", async (GameSummary gameSummary, IHubContext<LiveHub> hubContext) =>
        {
            logger.LogInformation("Received game ended {type} {gameid}", gameSummary.GameType, gameSummary.Id);
            await hubContext.Clients.Group(gameSummary.GameType).SendAsync("GameCompleted", gameSummary);
            return TypedResults.Ok();
        })
        .WithName("ReportGameEnded")
        .WithSummary("Report game ended to notify connected clients")
        .WithOpenApi();
    }
}
