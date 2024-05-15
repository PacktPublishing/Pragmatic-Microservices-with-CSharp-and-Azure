using Microsoft.Extensions.Caching.Distributed;

namespace Codebreaker.GameAPIs.Data.InMemory;

public class DistributedMemoryGamesRepository(IDistributedCache distributedCache, ILogger<GamesMemoryRepository> logger) : IGamesRepository
{
    public async Task AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        await UpdateGameInCacheAsync(game, cancellationToken);
    }

    public async Task<bool> DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await distributedCache.RemoveAsync(id.ToString(), cancellationToken);
        return true;
    }

    public async Task<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Game? game = await GetGameFromCacheAsync(id, cancellationToken: cancellationToken);
        return game;
    }

    public async Task AddMoveAsync(Game game, Move _, CancellationToken cancellationToken = default)
    {
        await UpdateGameInCacheAsync(game, cancellationToken: cancellationToken);
    }

    public Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> filteredGames = [];

        if (!string.IsNullOrEmpty(gamesQuery.PlayerName))
        {
            filteredGames = filteredGames.Where(g => g.PlayerName.Equals(gamesQuery.PlayerName));
        }

        if (gamesQuery.Date != null)
        {
            filteredGames = filteredGames.Where(g => DateOnly.FromDateTime(g.StartTime) == gamesQuery.Date);
        }

        if (gamesQuery.RunningOnly)
        {
            filteredGames = filteredGames.Where(g => !g.HasEnded());
        }

        if (gamesQuery.Ended)
        {
            filteredGames = filteredGames.Where(g => g.HasEnded());
        }

        filteredGames = filteredGames.OrderBy(g => g.Duration).ThenBy(g => g.StartTime).Take(500);

        filteredGames = filteredGames.ToList();

        return Task.FromResult(filteredGames);
    }

    public async Task<Game> UpdateGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        await UpdateGameInCacheAsync(game, cancellationToken: cancellationToken);
        return game;
    }

    private async Task<Game?> GetGameFromCacheAsync(Guid id, bool noCache = false, CancellationToken cancellationToken = default)
    {
        byte[]? bytesGame = await distributedCache.GetAsync(id.ToString(), cancellationToken);
        if (bytesGame is null)
        {
            logger.GameNotFound(id);
            return null;
        }
        else
        {
            return bytesGame.ToGame();
        }
    }

    private async Task UpdateGameInCacheAsync(Game game, CancellationToken cancellationToken = default)
    {
        // DistributedMemoryGamesRepository needs a different sliding expiration
        // as games are not persisted
        // One move up to - 15 minutes
        DistributedCacheEntryOptions options = new()
        {
             SlidingExpiration = TimeSpan.FromMinutes(15)
        };
        await distributedCache.SetAsync(game.Id.ToString(), game.ToBytes(), options, cancellationToken);
    }
}
