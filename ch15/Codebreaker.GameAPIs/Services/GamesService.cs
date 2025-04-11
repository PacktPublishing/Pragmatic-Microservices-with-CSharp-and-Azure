using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics;

namespace Codebreaker.GameAPIs.Services;

/// <summary>
/// Handles game operations such as starting, setting moves, retrieving, deleting, and ending games asynchronously.
/// </summary>
/// <param name="dataRepository">Used for storing and retrieving game data from a persistent storage.</param>
/// <param name="distributedCache">Facilitates caching game data to improve retrieval performance.</param>
/// <param name="liveClient">Responsible for reporting game status updates to an external service.</param>
/// <param name="logger">Logs various events and errors occurring during game operations.</param>
/// <param name="metrics">Tracks and records metrics related to game activities and performance.</param>
/// <param name="activitySource">Creates and manages activity tracing for monitoring operations.</param>
public class GamesService(
    IGamesRepository dataRepository, 
    IDistributedCache distributedCache, 
    IGameReport liveClient, 
    ILogger<GamesService> logger, 
    GamesMetrics metrics, 
    [FromKeyedServices("Codebreaker.GameAPIs")] ActivitySource activitySource) : IGamesService
{
    private const string GameTypeTagName = "codebreaker.gameType";
    private const string GameIdTagName = "codebreaker.gameId";

    /// <summary>
    /// Starts a new game asynchronously based on the specified type and player name.
    /// </summary>
    /// <param name="gameType">Specifies the type of game to be created.</param>
    /// <param name="playerName">Indicates the name of the player participating in the game.</param>
    /// <param name="cancellationToken">Allows for the operation to be canceled if needed.</param>
    /// <returns>Returns the newly created game instance.</returns>
    public async Task<Game> StartGameAsync(string gameType, string playerName, CancellationToken cancellationToken = default)
    {
        Game game;
        using var activity = activitySource.CreateActivity("StartGame", ActivityKind.Server);
        try
        {
            game = GamesFactory.CreateGame(gameType, playerName);
            activity?.AddTag(GameTypeTagName, game.GameType)
                .AddTag(GameIdTagName, game.Id.ToString())
                .Start();

            await Task.WhenAll(
                dataRepository.AddGameAsync(game, cancellationToken), 
                UpdateGameInCacheAsync(game, cancellationToken));

            metrics.GameStarted(game);
            logger.GameStarted(game.Id);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (CodebreakerException ex) when (ex.Code is CodebreakerExceptionCodes.InvalidGameType)
        {
            logger.InvalidGameType(gameType);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        return game;
    }

    /// <summary>
    /// Sets a move in a game based on the provided guesses and updates the game state accordingly.
    /// </summary>
    /// <param name="id">Identifies the specific game instance to update.</param>
    /// <param name="gameType">Specifies the type of game being played.</param>
    /// <param name="guesses">Contains the player's guesses for the current move.</param>
    /// <param name="moveNumber">Indicates the sequence number of the move being made.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>Returns a tuple containing the updated game and the move that was applied.</returns>
    public async Task<(Game Game, Move Move)> SetMoveAsync(Guid id, string gameType, string[] guesses, int moveNumber, CancellationToken cancellationToken = default)
    {
        Game? game = default;
        using var activity = activitySource.CreateActivity("SetMove", ActivityKind.Server);
        Move? move;
        try
        {
            game = await GetGameFromCacheOrDataStoreAsync(id, noCache: false, cancellationToken: cancellationToken);

            CodebreakerException.ThrowIfNull(game);
            CodebreakerException.ThrowIfEnded(game);
            CodebreakerException.ThrowIfUnexpectedGameType(game, gameType);

            activity?.AddTag(GameTypeTagName, game.GameType);
            activity?.AddTag(GameIdTagName, game.Id.ToString());
            activity?.Start();

            move = game.ApplyMove(guesses, moveNumber);

            // Update the game in the game-service database
            await Task.WhenAll(
                dataRepository.AddMoveAsync(game, move, cancellationToken), 
                UpdateGameInCacheAsync(game, cancellationToken));

            metrics.MoveSet(game.Id, DateTime.UtcNow, game.GameType);
            if (game.HasEnded())
            {
                logger.GameEnded(game);
                metrics.GameEnded(game);
                await liveClient.ReportGameEndedAsync(game.ToGameSummary(), cancellationToken);
            }
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (ArgumentException ex)
        {
            logger.InvalidMoveReceived(game?.Id ?? Guid.Empty, string.Join(':', guesses), ex.Message);
            metrics.InvalidMove();
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        return (game, move);
    }

    /// <summary>
    /// Retrieves a game asynchronously based on its unique identifier. If the game is not found, it logs a message.
    /// </summary>
    /// <param name="id">The unique identifier used to locate the specific game in the data store.</param>
    /// <param name="cancellationToken">Used to signal cancellation of the asynchronous operation if needed.</param>
    /// <returns>Returns the game object if found, otherwise returns null.</returns>
    public async ValueTask<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var game = await GetGameFromCacheOrDataStoreAsync(id, cancellationToken: cancellationToken);
        if (game is null)
        {
            logger.GameNotFound(id);
        }
        else
        {
            logger.QueryGame(game.Id);
        }
        return game;
    }

    /// <summary>
    /// Asynchronously deletes a game identified by a unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier for the game to be deleted.</param>
    /// <param name="cancellationToken">Used to signal cancellation of the delete operation if needed.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _ = distributedCache.RemoveAsync(id.ToString(), cancellationToken);
        await dataRepository.DeleteGameAsync(id, cancellationToken);
    }

    /// <summary>
    /// Ends a game by updating its end time and duration, then reports the game's conclusion.
    /// </summary>
    /// <param name="id">Identifies the specific game to be ended.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>Returns the updated game object after it has been ended.</returns>
    public async Task<Game> EndGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Game? game = await GetGameFromCacheOrDataStoreAsync(id, cancellationToken: cancellationToken);
        CodebreakerException.ThrowIfNull(game);

        game.EndTime = DateTime.UtcNow;
        TimeSpan duration = game.EndTime.Value - game.StartTime;
        game.Duration = duration;
        metrics.GameEnded(game);
        game = await dataRepository.UpdateGameAsync(game, cancellationToken);
        await Task.WhenAll(
            distributedCache.RemoveAsync(id.ToString(), cancellationToken),
            liveClient.ReportGameEndedAsync(game.ToGameSummary(), cancellationToken));
        return game;
    }

    /// <summary>
    /// Retrieves a list of games asynchronously based on the specified query parameters.
    /// </summary>
    /// <param name="gamesQuery">Specifies the criteria for filtering the games to be retrieved.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>An asynchronous collection of games that match the provided query.</returns>
    public async Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default)
    {
        var games = await dataRepository.GetGamesAsync(gamesQuery, cancellationToken);
        logger.QueryGames(games, gamesQuery.ToString());
        return games;
    }

    private async Task<Game?> GetGameFromCacheOrDataStoreAsync(Guid id, bool noCache = false, CancellationToken cancellationToken = default)
    {
        if (noCache)
        {
            return await dataRepository.GetGameAsync(id, cancellationToken);
        }
        else
        {
            byte[]? bytesGame = await distributedCache.GetAsync(id.ToString(), cancellationToken);
            if (bytesGame is null)
            {
                return await dataRepository.GetGameAsync(id, cancellationToken);
            }
            else
            {
                return bytesGame.ToGame();
            }
        }
    }

    private async Task UpdateGameInCacheAsync(Game game, CancellationToken cancellationToken = default)
    {
        await distributedCache.SetAsync(game.Id.ToString(), game.ToBytes(), cancellationToken);
    }
}
