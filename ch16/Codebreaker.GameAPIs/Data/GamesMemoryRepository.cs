using System.Collections.Concurrent;

namespace Codebreaker.GameAPIs.Data.InMemory;

public partial class GamesMemoryRepository(ILogger<GamesMemoryRepository> logger) : IGamesRepository
{
    private readonly ConcurrentDictionary<Guid, Game> _games = new();
    private DateTime _lastCleanupRun = DateTime.MinValue;
    private bool _cleanupRunnerActive = false;
    private static readonly object _cleanupLock = new();

    public Task AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        if (!_games.TryAdd(game.Id, game))
        {
            Log.GameExists(logger, game.Id);
            
        }
        _ = CleanupOldGamesAsync(); // don't need to wait for this to complete

        return Task.CompletedTask;
    }

    public Task<bool> DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_games.TryRemove(id, out _))
        {
            Log.GameNotFound(logger, id);
            return Task.FromResult(false);
        }
        return Task.FromResult(true);
    }

    public Task<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _games.TryGetValue(id, out Game? game);
        return Task.FromResult(game);
    }

    public Task AddMoveAsync(Game game, Move _, CancellationToken cancellationToken = default)
    {
        _games[game.Id] = game;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> filteredGames = _games.Values;

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

        _ = CleanupOldGamesAsync(); // don't need to wait for this to complete

        return Task.FromResult(filteredGames);
    }

    public Task<Game> UpdateGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        _games[game.Id] = game;
        return Task.FromResult(game);
    }

    private Task CleanupOldGamesAsync()
    {
        if (_lastCleanupRun > DateTime.Now.AddHours(-1))
        {
            return Task.CompletedTask;
        }
        lock (_cleanupLock)
        {
            if (!_cleanupRunnerActive)
            {
                return Task.CompletedTask;
            }
            _cleanupRunnerActive = true;
        }
        return Task.Run(() =>
        {
            _lastCleanupRun = DateTime.Now;

            logger.StartCleanupGames();
            var currentTime = DateTime.Now;
            var gamesToRemove = _games.Values.Where(g => g.StartTime <= currentTime.AddHours(-3)).ToList();
            int gamesRemoved = 0;
            foreach (var game in gamesToRemove)
            {
                if (_games.TryRemove(game.Id, out _))
                {
                    gamesRemoved++;
                }
            }
            logger.CleanedUpGames(gamesRemoved);
            lock (_cleanupLock)
            {
                _cleanupRunnerActive = false;
            }
        });
    }
}
