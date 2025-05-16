using Codebreaker.GameAPIs.Infrastructure;

using System.Diagnostics;

namespace Codebreaker.GameAPIs.Services;

public class GamesService(
    IGamesRepository dataRepository, 
    ILogger<GamesService> logger, 
    GamesMetrics metrics,
    [FromKeyedServices("Codebreaker.GameAPIs")] ActivitySource activitySource, 
    ILiveReportClient? liveReportClient = null) : IGamesService
{
    private const string GameTypeTagName = "codebreaker.gameType";
    private const string GameIdTagName = "codebreaker.gameId";

    /// <summary>
    /// Starts a new game of the specified type with the given player name.
    /// </summary>
    /// <remarks>This method creates a new game instance, persists it to the data repository, and logs
    /// relevant metrics and activity information.</remarks>
    /// <param name="gameType">The type of game to start. This must be a valid game type supported by the system.</param>
    /// <param name="playerName">The name of the player who will participate in the game.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Game"/> object representing the newly created game.</returns>
    public virtual async Task<Game> StartGameAsync(string gameType, string playerName, CancellationToken cancellationToken = default)
    {
        Game game;
        using var activity = activitySource.CreateActivity("StartGame", ActivityKind.Server);
        try
        {
            game = GamesFactory.CreateGame(gameType, playerName);
            activity?.AddTag(GameTypeTagName, game.GameType)
                .AddTag(GameIdTagName, game.Id.ToString())
                .Start();

            await dataRepository.AddGameAsync(game, cancellationToken);

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
    /// Processes a move for the specified game, validates the move, and updates the game state.
    /// </summary>
    /// <remarks>This method validates the game state and the provided move before applying it. If the game
    /// has ended or the game type does not match, an exception is thrown. The move is then persisted to the data
    /// repository, and metrics are updated accordingly.</remarks>
    /// <param name="id">The unique identifier of the game.</param>
    /// <param name="gameType">The type of the game, which must match the game's expected type.</param>
    /// <param name="guesses">An array of guesses representing the player's move.</param>
    /// <param name="moveNumber">The sequential number of the move being made.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A tuple containing the updated <see cref="Game"/> object and the <see cref="Move"/> object representing the
    /// processed move.</returns>
    public virtual async Task<(Game Game, Move Move)> SetMoveAsync(Guid id, string gameType, string[] guesses, int moveNumber, CancellationToken cancellationToken = default)
    {
        Game? game = default;
        using var activity = activitySource.CreateActivity("SetMove", ActivityKind.Server);
        Move? move;
        try
        {
            game = await GetGameAsync(id, cancellationToken: cancellationToken);

            CodebreakerException.ThrowIfNull(game);
            CodebreakerException.ThrowIfEnded(game);
            CodebreakerException.ThrowIfUnexpectedGameType(game, gameType);

            activity?.AddTag(GameTypeTagName, game.GameType);
            activity?.AddTag(GameIdTagName, game.Id.ToString());
            activity?.Start();

            move = game.ApplyMove(guesses, moveNumber);

            // Update the game in the game-service database
            await dataRepository.AddMoveAsync(game, move, cancellationToken);

            metrics.MoveSet(game.Id, DateTime.UtcNow, game.GameType);
            if (game.HasEnded())
            {
                TimeSpan duration = game.Duration ??= DateTime.UtcNow - game.StartTime;
                GameSummary gameSummary = new(game.Id, game.GameType.ToString(), game.PlayerName, true, game.IsVictory, game.LastMoveNumber, game.StartTime, duration);
                liveReportClient?.ReportGameEndedAsync(gameSummary, cancellationToken);
                logger.GameEnded(game);
                metrics.GameEnded(game);
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

    public virtual async ValueTask<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var game = await dataRepository.GetGameAsync(id, cancellationToken);
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

    public virtual async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await dataRepository.DeleteGameAsync(id, cancellationToken);
    }

    public virtual async Task<Game> EndGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Game? game = await GetGameAsync(id, cancellationToken: cancellationToken);
        CodebreakerException.ThrowIfNull(game);

        game.EndTime = DateTime.UtcNow;
        TimeSpan duration = game.EndTime.Value - game.StartTime;
        game.Duration = duration;
        metrics.GameEnded(game);
        game = await dataRepository.UpdateGameAsync(game, cancellationToken);
        return game;
    }

    public async Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default)
    {
        var games = await dataRepository.GetGamesAsync(gamesQuery, cancellationToken);
        logger.QueryGames(games, gamesQuery.ToString());
        return games;
    }
}
