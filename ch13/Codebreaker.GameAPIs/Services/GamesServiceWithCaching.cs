using Codebreaker.GameAPIs.Infrastructure;

using Microsoft.Extensions.Caching.Distributed;

using System.Diagnostics;

namespace Codebreaker.GameAPIs.Services;

public class GamesServiceWithCaching(
    IGamesRepository dataRepository, 
    IDistributedCache distributedCache, 
    ILogger<GamesService> logger, 
    GamesMetrics metrics, 
    [FromKeyedServices("Codebreaker.GameAPIs")] ActivitySource activitySource, 
    ILiveReportClient? liveReportClient = default) : 
    GamesService(dataRepository, logger, metrics, activitySource, liveReportClient)
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
