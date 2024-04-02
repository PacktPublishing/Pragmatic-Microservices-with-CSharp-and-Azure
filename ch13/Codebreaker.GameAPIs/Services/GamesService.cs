using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics;

namespace Codebreaker.GameAPIs.Services;

public class GamesService(IGamesRepository dataRepository, IDistributedCache distributedCache, ILiveReportClient liveClient, ILogger<GamesService> logger, GamesMetrics metrics, [FromKeyedServices("Codebreaker.GameAPIs")] ActivitySource activitySource) : IGamesService
{
    private const string GameTypeTagName = "codebreaker.gameType";
    private const string GameIdTagName = "codebreaker.gameId";

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

    // get the game from the cache or the data repository
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

    public async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _ = distributedCache.RemoveAsync(id.ToString(), cancellationToken);
        await dataRepository.DeleteGameAsync(id, cancellationToken);
    }

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
