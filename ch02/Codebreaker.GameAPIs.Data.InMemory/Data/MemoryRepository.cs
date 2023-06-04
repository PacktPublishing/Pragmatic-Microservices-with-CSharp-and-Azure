using Codebreaker.GameAPIs.Models;

namespace Codebreaker.GameAPIs.Data.InMemory;
public class MemoryRepository : ICodebreakerRepository
{
    private readonly Dictionary<Guid, Game> _games = new();

    public Task AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        _games.Add(game.GameId, game);
        return Task.CompletedTask;
    }

    public Task DeleteGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        _games.Remove(gameId);
        return Task.CompletedTask;
    }

    public Task<Game?> GetGameAsync(Guid gameId, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        _games.TryGetValue(gameId, out var game);
        return Task.FromResult(game);
    }

    public Task<IEnumerable<Game>> GetGamesByDateAsync(GameType gameType, DateOnly date, CancellationToken cancellationToken = default)
    {
        var games = _games.Values.Where(g => DateOnly.FromDateTime(g.StartTime) == date).ToArray();
        return Task.FromResult<IEnumerable<Game>>(games);
    }

    public Task<IEnumerable<Game>> GetMyGamesAsync(string playerName, CancellationToken cancellationToken = default)
    {
        var games = _games.Values.Where(g => g.PlayerName == playerName).ToArray();
        return Task.FromResult<IEnumerable<Game>>(games);
    }

    public Task<IEnumerable<Game>> GetMyRunningGamesAsync(string playerName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        _games[game.GameId] = game;
        return Task.CompletedTask;
    }
}
