using System.Collections.Concurrent;

namespace Codebreaker.GameAPIs.Data.InMemory;
public class GamesMemoryRepository(ILogger<GamesMemoryRepository> logger) : IGamesRepository
{
    private readonly ConcurrentDictionary<Guid, Game> _games = new();
    private readonly ILogger _logger = logger;

    public Task AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        if (!_games.TryAdd(game.GameId, game))
        {
            _logger.LogWarning("gameid {gameId} already exists", game.GameId);
        }
        return Task.CompletedTask;
    }

    public Task DeleteGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        if (!_games.TryRemove(gameId, out _))
        {
            _logger.LogWarning("gamid {gameId} not available", gameId);
        }
        return Task.CompletedTask;
    }

    public Task<Game?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        _games.TryGetValue(gameId, out Game? game);
        return Task.FromResult(game);
    }

    public Task<IEnumerable<Game>> GetGamesByDateAsync(string gameType, DateOnly date, CancellationToken cancellationToken = default)
    {
        var games = _games.Values.Where(g => DateOnly.FromDateTime(g.StartTime) == date).ToArray();
        return Task.FromResult<IEnumerable<Game>>(games);
    }

    public Task<IEnumerable<Game>> GetGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default)
    {
        var games = _games.Values.Where(g => g.PlayerName == playerName).ToArray();
        return Task.FromResult<IEnumerable<Game>>(games);
    }

    public Task<IEnumerable<Game>> GetRunningGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default)
    {
       
        var games = _games.Values
            .Where(g => g.PlayerName == playerName && g.StartTime >= DateTime.Today.AddDays(-1) && !g.Ended())
            .ToArray();
        return Task.FromResult<IEnumerable<Game>>(games);
    }

    public Task AddMoveAsync(Game game, Move move, CancellationToken cancellationToken = default)
    {
        _games[game.GameId] = game;
        return Task.CompletedTask;
    }
}
