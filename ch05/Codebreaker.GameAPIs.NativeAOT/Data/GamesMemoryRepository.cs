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

    public Task<bool> DeleteGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        if (!_games.TryRemove(gameId, out _))
        {
            _logger.LogWarning("gamid {gameId} not available", gameId);
            return Task.FromResult(false);
        }
        return Task.FromResult(true);
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

    public Task<IEnumerable<Game>> GetGamesAsync(GamesQuery? gamesQuery = null, CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> filteredGames = _games.Values;

        if (gamesQuery != null)
        {
            if (!string.IsNullOrEmpty(gamesQuery.GameType))
            {
                filteredGames = filteredGames.Where(g => g.GameType == gamesQuery.GameType);
            }

            if (gamesQuery.Date != null)
            {
                filteredGames = filteredGames.Where(g => DateOnly.FromDateTime(g.StartTime.Date) == gamesQuery.Date);
            }

            if (!string.IsNullOrEmpty(gamesQuery.PlayerName))
            {
                filteredGames = filteredGames
                    .Where(g => g.PlayerName == gamesQuery.PlayerName);
            }

            if (gamesQuery.Ended == false)
            {
                filteredGames = filteredGames
                    .Where(g => !g.Ended());
            }

            if (gamesQuery.Ended == true)
            {
                filteredGames = filteredGames
                    .Where(g => g.Ended())
                    .OrderBy(g => g.Duration);
            }
            else
            {
                filteredGames = filteredGames
                    .OrderByDescending(g => g.StartTime);
            }

        }

        return Task.FromResult(filteredGames.AsEnumerable());
    }

    public Task<Game> UpdateGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        _games.TryGetValue(game.GameId, out var existingGame);
        CodebreakerException.ThrowIfNull(existingGame);

        if (_games.TryUpdate(game.GameId, game, existingGame))
        {
            return Task.FromResult(game);
        }

        throw new CodebreakerException($"Game update failed with game id {game.GameId}")
        {
            Code = CodebreakerExceptionCodes.GameUpdateFailed
        };
    }
}
