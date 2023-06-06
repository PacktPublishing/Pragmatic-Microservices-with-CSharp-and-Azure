using Codebreaker.GameAPIs.Exceptions;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Codebreaker.GameAPIs.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/games")
            .WithTags("Codebreaker");

        // Create game
        group.MapPost("/", async Task<Results<Created<CreateGameResponse>, BadRequest<string>>> (
            CreateGameRequest request,
            IGamesService gameService,
            CancellationToken cancellationToken) =>
        {
            Game game;
            try
            {
                game = await gameService.StartGameAsync(request.GameType.ToString(), request.PlayerName, cancellationToken);
            }
            catch (GameTypeNotFoundException)
            {
                return TypedResults.BadRequest("Gametype does not exist");
            }
            return TypedResults.Created($"/games/{game.GameId}", game.ToCreateGameResponse());
        })
        .WithName("CreateGame")
        .WithSummary("Creates and starts a game")
        .WithOpenApi(op =>
        {
            op.RequestBody.Description = "The data of the game to create";
            return op;
        });

        // Update the game resource with a move
        group.MapPut("/{gameId:guid}/moves", async Task<Results<Ok<SetMoveResponse>, NotFound, BadRequest<string>>> (
            Guid gameId,
            SetMoveRequest request,
            IGamesService gameService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                Game game = await gameService.SetMoveAsync(gameId, request.GuessPegs, request.MoveNumber, cancellationToken);
                return TypedResults.Ok(game.ToSetMoveResponse());
            }
            catch (GameNotFoundException)
            {
                return TypedResults.NotFound();
            }
        })
        .WithName("SetMove")
        .WithSummary("Sets a move for the game with the given id")
        .WithOpenApi(op =>
        {
            op.Parameters[0].Description = "The id of the game to create a move for";
            op.RequestBody.Description = "The data for creating the move";
            return op;
        });

        group.MapGet("/rank/{date}", async (
            DateOnly date,
            GameType gameType,
            IGamesService gameService,
            CancellationToken cancellationToken
        ) =>
        {
            IEnumerable<Game> games = await gameService.GetGamesRankByDateAsync(gameType, date, cancellationToken);

            return TypedResults.Ok(games.ToGamesRankResponse(date, gameType));
        })
        .WithName("GetGames")
        .WithSummary("Get games by the given date and type")
        .WithOpenApi(op =>
        {
            op.Parameters[0].Description = "The of date to get the games from. (e.g. 2023-01-01)";
            return op;
        });

        // Get game by id
        group.MapGet("/{gameId:guid}", async Task<Results<Ok<Game>, NotFound>> (
            Guid gameId,
            IGamesService gameService
        ) =>
        {
            Game? game = await gameService.GetGameAsync(gameId);

            if (game is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(game);
        })
        .WithName("GetGame")
        .WithSummary("Gets a game by the given id")
        .WithOpenApi(op =>
        {
            op.Parameters[0].Description = "The id of the game to get";
            return op;
        });

        group.MapDelete("/{gameId:guid}", async (
            Guid gameId,
            IGamesService gameService
        ) =>
        {
            await gameService.DeleteGameAsync(gameId);

            return TypedResults.NoContent();
        })
        .WithName("DeleteGame")
        .WithSummary("Deletes the game with the given id")
        .WithDescription("Deletes a game from the database")
        .WithOpenApi(op =>
        {
            op.Parameters[0].Description = "The id of the game to delete or cancel";
            return op;
        });
    }
}
