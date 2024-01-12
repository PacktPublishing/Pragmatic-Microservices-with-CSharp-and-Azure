using System.Collections.Concurrent;

namespace Codebreaker.GameAPIs.Data.InMemory;

public partial class GamesMemoryRepository(ILogger<GamesMemoryRepository> logger) : IGamesRepository
{
    private readonly ConcurrentDictionary<Guid, Game> _games = new();

    public Task AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        if (!_games.TryAdd(game.Id, game))
        {
            Log.GameExists(logger, game.Id);
            
        }
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

    public Task AddMoveAsync(Game game, Move move, CancellationToken cancellationToken = default)
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
            filteredGames = filteredGames.Where(g => !g.Ended());
        }

        if (gamesQuery.Ended)
        {
            filteredGames = filteredGames.Where(g => g.Ended());
        }

        return Task.FromResult(filteredGames);
    }

    public Task<Game> UpdateGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        _games.TryGetValue(game.Id, out var existingGame);
        CodebreakerException.ThrowIfNull(existingGame);

        if (_games.TryUpdate(game.Id, game, existingGame))
        {
            return Task.FromResult(game);
        }

        throw new CodebreakerException($"Game update failed with game id {game.Id}") 
        { 
            Code = CodebreakerExceptionCodes.GameUpdateFailed 
        };
    }
}
