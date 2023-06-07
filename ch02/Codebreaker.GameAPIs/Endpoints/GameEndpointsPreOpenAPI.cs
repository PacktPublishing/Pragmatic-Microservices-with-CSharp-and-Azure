using Codebreaker.GameAPIs.Exceptions;

namespace Codebreaker.GameAPIs.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/games");

        // Create game
        //group.MapPost("/", async (
        //    CreateGameRequest request,
        //    IGamesService gameService,
        //    CancellationToken cancellationToken) =>
        //{
        //    Game game;
        //    try
        //    {
        //        game = await gameService.StartGameAsync(request.GameType.ToString(), request.PlayerName, cancellationToken);
        //    }
        //    catch (GameTypeNotFoundException)
        //    {
        //        return Results.BadRequest("Gametype does not exist");
        //    }
        //    return Results.Created($"/games/{game.GameId}", game.ToCreateGameResponse());
        //});

        group.MapPost("/", async (
            CreateGameRequest request,
            IGamesService gameService,
            CancellationToken cancellationToken) =>
        {
            Game game = await gameService.StartGameAsync(request.GameType.ToString(), request.PlayerName, cancellationToken);
            return Results.Created($"/games/{game.GameId}", game.ToCreateGameResponse());
        }).AddEndpointFilter<ValidatePlayernameEndpointFilter>()
        .AddEndpointFilter<CreateGameExceptionEndpointFilter>();


        // Get game by id
        group.MapGet("/{gameId:guid}", async (
            Guid gameId,
            IGamesService gameService
        ) =>
        {
            Game? game = await gameService.GetGameAsync(gameId);

            if (game is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(game);
        });

        // Update the game resource with a move
        group.MapPatch("/{gameId:guid}/moves", async (
            Guid gameId,
            SetMoveRequest request,
            IGamesService gameService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                (Game game, string result) = await gameService.SetMoveAsync(gameId, request.GuessPegs, request.MoveNumber, cancellationToken);
                return Results.Ok(game.ToSetMoveResponse(result));
            }
            catch (GameNotFoundException)
            {
                return Results.NotFound();
            }
        });

        group.MapGet("/rank/{date}", async (
            DateOnly date,
            GameType gameType,
            IGamesService gameService,
            CancellationToken cancellationToken
        ) =>
        {
            IEnumerable<Game> games = await gameService.GetGamesRankByDateAsync(gameType, date, cancellationToken);

            return Results.Ok(games.ToGamesRankResponse(date, gameType));
        });

        group.MapDelete("/{gameId:guid}", async (
            Guid gameId,
            IGamesService gameService
        ) =>
        {
            await gameService.DeleteGameAsync(gameId);

            return Results.NoContent();
        });
    }
}
