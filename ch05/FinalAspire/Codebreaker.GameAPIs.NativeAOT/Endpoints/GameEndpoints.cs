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
                GameError error = new(ErrorCodes.InvalidGameType, $"Game type {request.GameType} does not exist", context.Request.GetDisplayUrl(), Enum.GetNames<GameType>());
                return TypedResults.BadRequest(error);
            }
            return TypedResults.Created($"/games/{game.Id}", game.AsCreateGameResponse());
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
            catch (ArgumentException ex) when (ex.HResult is <= 4200 and >= 400)
            {
                string url = context.Request.GetDisplayUrl();
                return ex.HResult switch
                {
                    4200 => TypedResults.BadRequest(new GameError(ErrorCodes.InvalidGuessNumber, "Invalid number of guesses received", url)),
                    4300 => TypedResults.BadRequest(new GameError(ErrorCodes.UnexpectedMoveNumber, "Unexpected move number received", url)),
                    4400 => TypedResults.BadRequest(new GameError(ErrorCodes.InvalidGuess, "Invalid guess values received!", url)),
                    _ => TypedResults.BadRequest(new GameError(ErrorCodes.InvalidMove, "Invalid move received!", url))
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
        });

        // Get game by id
        group.MapGet("/{id:guid}", async Task<Results<Ok<Game>, NotFound>> (
            Guid id,
            IGamesService gameService,
            CancellationToken cancellationToken
        ) =>
        {
            Game? game = await gameService.GetGameAsync(id, cancellationToken);

            if (game is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(game);
        });

        // TODO: bool and CancellationToken parameters have an issue with the RequestDelegateGenerator using .NET 8 RC 1; update with .NET 8 Update 1
        group.MapGet("/", async (
            IGamesService gameService,
            CancellationToken cancellationToken,
            string ? gameType = default,
            string? playerName = default,
            DateOnly? date = default,
            bool? ended = default) =>
        {
            GamesQuery query = new(gameType, playerName, date, Ended: ended ?? false);
            var games = await gameService.GetGamesAsync(query, cancellationToken);
            return TypedResults.Ok(games);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IGamesService gameService,
            CancellationToken cancellationToken
        ) =>
        {
            await gameService.DeleteGameAsync(id, cancellationToken);

            return TypedResults.NoContent();
        });
    }
}
