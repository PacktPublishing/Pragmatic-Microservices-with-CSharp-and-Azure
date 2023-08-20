using Codebreaker.GameAPIs.Errors;

using Microsoft.AspNetCore.Http.Extensions;

namespace Codebreaker.GameAPIs.Endpoints;

public static class GameEndpoints1
{
    public static void MapGameEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/games");

        group.MapPost("/", async (
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
                GameError error = new(ErrorCodes.InvalidGameType, $"Game type {request.GameType} does not exist", context.Request.GetDisplayUrl(), Enum.GetNames<GameType>());
                return Results.BadRequest(error);
            }
            return Results.Created($"/games/{game.GameId}", game.AsCreateGameResponse());
        }).AddEndpointFilter<ValidatePlayernameEndpointFilter>()
        .AddEndpointFilter<CreateGameExceptionEndpointFilter>();

        // Update the game resource with a move
        group.MapPatch("/{gameId:guid}", async (
            Guid gameId,
            UpdateGameRequest request,
            IGamesService gameService,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            if (!request.End && request.GuessPegs == null)
            {
                return Results.BadRequest(new GameError(ErrorCodes.InvalidMove, "End the game or set guesses", context.Request.GetDisplayUrl()));
            }
            try
            {
                if (request.End)
                {
                    Game? game = await gameService.EndGameAsync(gameId, cancellationToken);
                    if (game is null)
                        return Results.NotFound();
                    return Results.Ok(game.AsUpdateGameResponse());
                }
                else
                {
                    (Game game, Move move) = await gameService.SetMoveAsync(gameId, request.GuessPegs!, request.MoveNumber, cancellationToken);
                    return Results.Ok(game.AsUpdateGameResponse(move.KeyPegs));
                }
            }
            catch (ArgumentException ex) when (ex.HResult is <= 4400 and >= 4000)
            {
                string url = context.Request.GetDisplayUrl();
                return ex.HResult switch
                {
                    4200 => Results.BadRequest(new GameError(ErrorCodes.InvalidGuessNumber, "Invalid number of guesses received", url)),
                    4300 => Results.BadRequest(new GameError(ErrorCodes.UnexpectedMoveNumber, "Unexpected move number received", url)),
                    4400 => Results.BadRequest(new GameError(ErrorCodes.InvalidGuess, "Invalid guess values received!", url)),
                    _ => Results.BadRequest(new GameError(ErrorCodes.InvalidMove, "Invalid move received!", url))
                };
            }
            catch (CodebreakerException ex) when (ex.Code == CodebreakerExceptionCodes.GameNotFound)
            {
                return Results.NotFound();
            }
            catch (CodebreakerException ex) when (ex.Code == CodebreakerExceptionCodes.GameNotActive)
            {
                string url = context.Request.GetDisplayUrl();
                return Results.BadRequest(new GameError(ErrorCodes.GameNotActive, "The game already ended", url));
            }
        });

        // Get game by id
        group.MapGet("/{gameId:guid}", async (
            Guid gameId,
            IGamesService gameService,
            CancellationToken cancellationToken) =>
        {
            Game? game = await gameService.GetGameAsync(gameId, cancellationToken);

            if (game is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(game);
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
            return Results.Ok(games);
        });

        group.MapDelete("/{gameId:guid}", async (
            Guid gameId,
            IGamesService gameService,
            CancellationToken cancellationToken) =>
        {
            await gameService.DeleteGameAsync(gameId, cancellationToken);

            return Results.NoContent();
        });
    }
}
