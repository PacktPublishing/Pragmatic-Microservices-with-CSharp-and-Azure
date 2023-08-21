namespace Codebreaker.GameAPIs.Services;

public class GamesService(IGamesRepository dataRepository) : IGamesService
{
    private readonly IGamesRepository _dataRepository = dataRepository;

    public async Task<Game> StartGameAsync(string gameType, string playerName, CancellationToken cancellationToken = default)
    {
        Game game = GamesFactory.CreateGame(gameType, playerName);

        await _dataRepository.AddGameAsync(game, cancellationToken);
        return game;
    }

    public async Task<(Game Game, Move Move)> SetMoveAsync(Guid gameId, string[] guesses, int moveNumber, CancellationToken cancellationToken = default)
    {
        Game? game = await _dataRepository.GetGameAsync(gameId, cancellationToken);
        CodebreakerException.ThrowIfNull(game);
        CodebreakerException.ThrowIfEnded(game);

        Move move = game.ApplyMove(guesses, moveNumber);

        // Update the game in the game-service database
        await _dataRepository.AddMoveAsync(game, move, cancellationToken);

        return (game, move);
    }

    // get the game from the cache or the data repository
    public async ValueTask<Game> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var game = await _dataRepository.GetGameAsync(gameId, cancellationToken);
        CodebreakerException.ThrowIfNull(game);
        return game;
    }

    public async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _dataRepository.DeleteGameAsync(id, cancellationToken);
    }

    public async Task<Game> EndGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        Game? game = await _dataRepository.GetGameAsync(gameId, cancellationToken);
        CodebreakerException.ThrowIfNull(game);
       
        game.EndTime = DateTime.Now;
        game.Duration = game.EndTime - game.StartTime;
        game = await _dataRepository.UpdateGameAsync(game, cancellationToken);
        return game;
    }

    public async Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default)
    {
        return await _dataRepository.GetGamesAsync(gamesQuery, cancellationToken);
    }
}
