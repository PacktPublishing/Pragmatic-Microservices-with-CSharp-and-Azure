using Codebreaker.GameAPIs.Errors;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Codebreaker.GameAPIs.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/games")
            .WithTags("Games API");

        group.MapPost("/", async Task<Results<Created<CreateGameResponse>, BadRequest<GameError>>> (
            CreateGameRequest request,
            IGamesService gameService,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            Game game;
            try
            {
                game = await gameService.StartGameAsync(request.GameType.ToString(), request.PlayerName, cancellationToken);
            }
            catch (CodebreakerException ex) when (ex.Code == CodebreakerExceptionCodes.InvalidGameType)
            {
                GameError error = new(ErrorCodes.InvalidGameType, $"Game type {request.GameType} does not exist", context.Request.GetDisplayUrl(),   Enum.GetNames<GameType>());
                return TypedResults.BadRequest(error);
            }
            return TypedResults.Created($"/games/{game.GameId}", game.AsCreateGameResponse());
        })
        .WithName("CreateGame")
        .WithSummary("Creates and starts a game")
        .WithOpenApi(op =>
        {
            op.RequestBody.Description = "The game type and the player name of the game to create";
            return op;
        });

        // Update the game resource with a move
        group.MapPatch("/{gameId:guid}", async Task<Results<Ok<UpdateGameResponse>, NotFound, BadRequest<GameError>>> (
            Guid gameId,
            UpdateGameRequest request,
            IGamesService gameService,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            if (!request.End && request.GuessPegs == null)
            {
                return TypedResults.BadRequest(new GameError(ErrorCodes.InvalidMove, "End the game or set guesses", context.Request.GetDisplayUrl()));
            }
            try
            {
                if (request.End)
                {
                    Game? game = await gameService.EndGameAsync(gameId, cancellationToken);
                    if (game is null)
                        return TypedResults.NotFound();
                    return TypedResults.Ok(game.AsUpdateGameResponse());
                }
                else
                {
                    (Game game, Move move) = await gameService.SetMoveAsync(gameId, request.GuessPegs!, request.MoveNumber, cancellationToken);
                    return TypedResults.Ok(game.AsUpdateGameResponse(move.KeyPegs));
                }
            }
            catch (ArgumentException ex) when (ex.HResult is >= 4200 and <= 4500)
            {
                string url = context.Request.GetDisplayUrl();
                return ex.HResult switch
                {
                    4200 => TypedResults.BadRequest(new GameError(ErrorCodes.InvalidGuessNumber, "Invalid number of guesses received", url)),
                    4300 => TypedResults.BadRequest(new GameError(ErrorCodes.UnexpectedMoveNumber, "Unexpected move number received", url)),
                    4400 => TypedResults.BadRequest(new GameError(ErrorCodes.InvalidGuess, "Invalid guess values received!", url)),
                    _ => TypedResults.BadRequest(new GameError(ErrorCodes.InvalidMove,"Invalid move received!", url))
                };
            }
            catch (CodebreakerException ex) when (ex.Code == CodebreakerExceptionCodes.GameNotFound)
            {
                return TypedResults.NotFound();
            }
            catch (CodebreakerException ex) when (ex.Code == CodebreakerExceptionCodes.GameNotActive)
            {
                string url = context.Request.GetDisplayUrl();
                return TypedResults.BadRequest(new GameError(ErrorCodes.GameNotActive, "The game already ended", url));
            }
        })
        .WithName("SetMove")
        .WithSummary("End the game or set a move")
        .WithOpenApi(op =>
        {
            op.Parameters[0].Description = "The id of the game to set a move";
            op.RequestBody.Description = "The data for creating the move";
            return op;
        });

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
        .WithSummary("Gets a game by the given id")
        .WithOpenApi(op =>
        {
            op.Parameters[0].Description = "The id of the game to get";
            return op;
        });

        group.MapGet("/", async (
            IGamesService gameService,
            string? gameType = default,
            string? playerName = default,
            DateOnly? date = default,
            bool ended = false,
            CancellationToken cancellationToken = default) =>
                {
                    GamesQuery query = new(gameType, playerName, date, Ended: ended);
                    var games = await gameService.GetGamesAsync(query, cancellationToken);
                    return TypedResults.Ok(games);
                })
                .WithName("GetGames")
                .WithSummary("Get games based on query parameters")
                .WithOpenApi(op =>
                {
                    op.Parameters[0].Description = "The game type to filter by";
                    op.Parameters[1].Description = "The player name to filter by";
                    op.Parameters[2].Description = "The date to filter by";
                    op.Parameters[3].Description = "Whether to filter by ended games";
                    return op;
                });

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
        .WithDescription("Deletes a game from the database")
        .WithOpenApi(op =>
        {
            op.Parameters[0].Description = "The id of the game to delete or cancel";
            return op;
        });
    }
}
