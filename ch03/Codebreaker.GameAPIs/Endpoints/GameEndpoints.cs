using Codebreaker.GameAPIs.Errors;
using Codebreaker.GameAPIs.Exceptions;

using Microsoft.AspNetCore.Http.HttpResults;

namespace Codebreaker.GameAPIs.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/games")
            .WithTags("Games API");

        group.MapPost("/", async Task<Results<Created<CreateGameResponse>, BadRequest<InvalidGameTypeError>>> (
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
                InvalidGameTypeError error = new($"Game type {request.GameType} does not exist", Enum.GetNames<GameType>());
                return TypedResults.BadRequest(error);
            }
            return TypedResults.Created($"/games/{game.GameId}", game.ToCreateGameResponse());
        })
#if NET8_0_OR_GREATER
        .WithOpenApi(op =>
        {
            op.RequestBody.Description = "The game type and the player name of the game to create";
            return op;
        })
#endif
        .WithName("CreateGame")
        .WithSummary("Creates and starts a game");

        // Update the game resource with a move
        group.MapPatch("/{gameId:guid}/moves", async Task<Results<Ok<SetMoveResponse>, NotFound, BadRequest<InvalidGameMoveError>>> (
            Guid gameId,
            SetMoveRequest request,
            IGamesService gameService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                (Game game, string result) = await gameService.SetMoveAsync(gameId, request.GuessPegs, request.MoveNumber, cancellationToken);
                return TypedResults.Ok(game.ToSetMoveResponse(result));
            }
            catch (ArgumentException ex) when (ex.HResult is <= 4200 and >= 400)
            {
                return ex.HResult switch
                {
                    4200 => TypedResults.BadRequest(new InvalidGameMoveError("Invalid number of guesses received")),
                    4300 => TypedResults.BadRequest(new InvalidGameMoveError("Invalid move number received")),
                    4400 => TypedResults.BadRequest(new InvalidGameMoveError("Invalid guess values received!")),
                    _ => TypedResults.BadRequest(new InvalidGameMoveError("Invalid move received!")),
                };
            }
            catch (GameNotFoundException)
            {
                return TypedResults.NotFound();
            }
        })
#if NET8_0_OR_GREATER
        .WithOpenApi(op =>
        {
            op.Parameters[0].Description = "The id of the game to create a move for";
            op.RequestBody.Description = "The data for creating the move";
            return op;
        })
#endif
        .WithName("SetMove")
        .WithSummary("Sets a move for the game with the given id");

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
        .WithSummary("Get games by the given date and type");
        //.WithOpenApi(op =>
        //{
        //    op.Parameters[0].Description = "The of date to get the games from. (e.g. 2023-01-01)";
        //    return op;
        //});

        // Get game by id
        group.MapGet("/{gameId:guid}", async Task<Results<Ok<Game>, NotFound>> (
            Guid gameId,
            IGamesService gameService,
            CancellationToken cancellationToken
        ) =>
        {
            Game? game = await gameService.GetGameAsync(gameId, cancellationToken);

            if (game is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(game);
        })
        .WithName("GetGame")
        .WithSummary("Gets a game by the given id");
        //.WithOpenApi(op =>
        //{
        //    op.Parameters[0].Description = "The id of the game to get";
        //    return op;
        //});

        group.MapDelete("/{gameId:guid}", async (
            Guid gameId,
            IGamesService gameService,
            CancellationToken cancellationToken
        ) =>
        {
            await gameService.DeleteGameAsync(gameId, cancellationToken);

            return TypedResults.NoContent();
        })
        .WithName("DeleteGame")
        .WithSummary("Deletes the game with the given id")
        .WithDescription("Deletes a game from the database");
        //.WithOpenApi(op =>
        //{
        //    op.Parameters[0].Description = "The id of the game to delete or cancel";
        //    return op;
        //});
    }
}
