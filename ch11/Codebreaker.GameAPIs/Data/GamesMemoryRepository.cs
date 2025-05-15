﻿using System.Collections.Concurrent;

namespace Codebreaker.GameAPIs.Data.InMemory;

public partial class GamesMemoryRepository(ILogger<GamesMemoryRepository> logger) : IGamesRepository
{
    private readonly ConcurrentDictionary<Guid, Game> _games = new();
    private readonly ILogger _logger = logger;

    public Task AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        if (!_games.TryAdd(game.Id, game))
        {
            _logger.LogWarning("id {id} already exists", game.Id);
        }
        return Task.CompletedTask;
    }

    public Task<bool> DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!_games.TryRemove(id, out _))
        {
            _logger.LogWarning("id {id} not available", id);
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
            filteredGames = filteredGames.Where(g => !g.HasEnded());
        }

        if (gamesQuery.Ended)
        {
            filteredGames = filteredGames.Where(g => g.HasEnded());
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
        _logger.LogError("Game update failed with game id {gameId}", game.Id);

        throw new CodebreakerException($"Game update failed with game id {game.Id}") 
        { 
            Code = CodebreakerExceptionCodes.GameUpdateFailed 
        };
    }
}
