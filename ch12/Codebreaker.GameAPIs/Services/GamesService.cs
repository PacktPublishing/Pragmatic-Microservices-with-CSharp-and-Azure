using Codebreaker.GameAPIs.Infrastructure;
using Codebreaker.ServiceDefaults;

using Microsoft.Extensions.Caching.Distributed;

using System.Diagnostics;

namespace Codebreaker.GameAPIs.Services;

public class GamesService(IGamesRepository dataRepository, ILogger<GamesService> logger, GamesMetrics metrics, [FromKeyedServices("Codebreaker.GameAPIs")] ActivitySource activitySource) : IGamesService
{
    private const string GameTypeTagName = "codebreaker.gameType";
    private const string GameIdTagName = "codebreaker.gameId";
    private readonly bool _useCache = !(Environment.GetEnvironmentVariable(EnvVarNames.Caching) == CachingType.None.ToString());

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

    // get the game from the the data repository
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

public class GamesServiceWithCaching(IGamesRepository dataRepository, IDistributedCache distributedCache, ILogger<GamesService> logger, GamesMetrics metrics, [FromKeyedServices("Codebreaker.GameAPIs")] ActivitySource activitySource) : GamesService(dataRepository, logger, metrics, activitySource)
{
    public override async Task<Game> StartGameAsync(string gameType, string playerName, CancellationToken cancellationToken = default)
    {
        Game game = await base.StartGameAsync(gameType, playerName, cancellationToken);
        await UpdateGameInCacheAsync(game, cancellationToken);

        return game;
    }

    public override async Task<(Game Game, Move Move)> SetMoveAsync(Guid id, string gameType, string[] guesses, int moveNumber, CancellationToken cancellationToken = default)
    {
        (Game game, Move move) = await base.SetMoveAsync(id, gameType, guesses, moveNumber, cancellationToken);
        await UpdateGameInCacheAsync(game, cancellationToken);       
        return (game, move);
    }

    // get the game from the cache or the data repository
    public override async ValueTask<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        byte[]? bytesGame = await distributedCache.GetAsync(id.ToString(), cancellationToken);
        if (bytesGame is null)
        {
            var game = await base.GetGameAsync(id, cancellationToken);
            if (game is not null)
            {
                await UpdateGameInCacheAsync(game, cancellationToken);
            }
            return game;
        }
        else
        {
            return bytesGame.ToGame();
        }
    }

    public override async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await distributedCache.RemoveAsync(id.ToString(), cancellationToken);
        await base.DeleteGameAsync(id, cancellationToken);
    }

    public override async Task<Game> EndGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Game game = await base.EndGameAsync(id, cancellationToken);
        await distributedCache.RemoveAsync(id.ToString(), cancellationToken);
        return game;
    }

    private async Task UpdateGameInCacheAsync(Game game, CancellationToken cancellationToken = default)
    {
        await distributedCache.SetAsync(game.Id.ToString(), game.ToBytes(), cancellationToken);
    }
}
