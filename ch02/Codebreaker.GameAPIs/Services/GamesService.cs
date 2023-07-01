using Codebreaker.GameAPIs.Data;
using Codebreaker.GameAPIs.Exceptions;

using Game = Codebreaker.GameAPIs.Models.Game;

namespace Codebreaker.GameAPIs.Services;

public class GamesService(IGamesRepository dataRepository) : IGamesService
{
    private readonly IGamesRepository _dataRepository = dataRepository;

    public async Task<Game> StartGameAsync(string gameType, string playerName, CancellationToken cancellationToken = default)
    {
        Game game = GamesFactory.CreateGame(gameType, playerName, isAuthenticated: false);

        await _dataRepository.AddGameAsync(game, cancellationToken);
        return game;
    }

    public async Task<(Game Game, string Result)> SetMoveAsync(Guid gameId, IEnumerable<string> guesses, int moveNumber, CancellationToken cancellationToken = default)
    {
        Game game = await _dataRepository.GetGameAsync(gameId, cancellationToken) ?? throw new GameNotFoundException($"Game with id {gameId} not found");

        (string result, Move move) = game.ApplyMove(guesses, moveNumber);

        // Update the game in the game-service database
        await _dataRepository.AddMoveAsync(game, move, cancellationToken);

        return (game, result);
    }

    // get the game from the cache or the data repository
    public async ValueTask<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var game = await _dataRepository.GetGameAsync(id, cancellationToken);
        return game;
    }

    public async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _dataRepository.DeleteGameAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Game>> GetMyGamesAsync(string playerName, CancellationToken cancellationToken = default)
    {
        var games = await _dataRepository.GetGamesByPlayerAsync(playerName, cancellationToken);
        return games;
    }

    public Task<Game> EndGameAsync(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Game>> GetRunningGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default)
    {
        return await _dataRepository.GetRunningGamesByPlayerAsync(playerName, cancellationToken);
    }

    public async Task<IEnumerable<Game>> GetAllMyGamesAsync(string playerName, CancellationToken cancellationToken = default)
    {
        return await _dataRepository.GetGamesByPlayerAsync(playerName, cancellationToken);
    }

    public async Task<IEnumerable<Game>> GetGamesRankByDateAsync(GameType gameType, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _dataRepository.GetGamesByDateAsync(gameType.ToString(), date, cancellationToken);      
    }

    public Task<Game> EndGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Game>> GetCompletedGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
