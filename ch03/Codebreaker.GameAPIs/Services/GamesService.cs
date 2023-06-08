using Codebreaker.GameAPIs.Analyzers;
using Codebreaker.GameAPIs.Contracts;
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

    private static string ApplyMove(Game game, IEnumerable<string> guesses, int moveNumber)
    {
        ColorGameMoveAnalyzer GetColorGameMoveAnalyzer(ColorGame game) =>
            new(game, guesses.ToPegs<ColorField>(), moveNumber);

        SimpleGameMoveAnalyzer GetSimpleGameMoveAnalyzer(SimpleGame game) =>
            new(game, guesses.ToPegs<ColorField>(), moveNumber);

        ShapeGameMoveAnalyzer GetShapeGameMoveAnalyzer(ShapeGame game) =>
            new(game, guesses.ToPegs<ShapeAndColorField>(), moveNumber);

        IGameMoveAnalyzer analyzer = game switch
        {
            ColorGame g => GetColorGameMoveAnalyzer(g),
            SimpleGame g => GetSimpleGameMoveAnalyzer(g),
            ShapeGame g => GetShapeGameMoveAnalyzer(g),
            _ => throw new NotImplementedException()
        };

        return analyzer.ApplyMove();
    }

    public async Task<(Game Game, string Result)> SetMoveAsync(Guid gameId, IEnumerable<string> guesses, int moveNumber, CancellationToken cancellationToken = default)
    {
        Game game = await _dataRepository.GetGameAsync(gameId, cancellationToken) ?? throw new GameNotFoundException($"Game with id {gameId} not found");

        string result = ApplyMove(game, guesses, moveNumber);

        // Update the game in the game-service database
        await _dataRepository.UpdateGameAsync(game, cancellationToken);

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
        var games = await _dataRepository.GetMyGamesAsync(playerName, cancellationToken);
        return games;
    }

    public Task<Game> EndGameAsync(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Game>> GetMyLastRunningGamesAsync(string playerName, CancellationToken cancellationToken = default)
    {
        return await _dataRepository.GetMyRunningGamesAsync(playerName, cancellationToken);
    }

    public async Task<IEnumerable<Game>> GetAllMyGamesAsync(string playerName, CancellationToken cancellationToken = default)
    {
        return await _dataRepository.GetMyGamesAsync(playerName, cancellationToken);
    }

    public async Task<IEnumerable<Game>> GetGamesRankByDateAsync(GameType gameType, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _dataRepository.GetGamesByDateAsync(gameType.ToString(), date, cancellationToken);      
    }
}
