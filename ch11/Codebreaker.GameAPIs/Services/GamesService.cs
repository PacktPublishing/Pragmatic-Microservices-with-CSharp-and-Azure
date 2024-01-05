using Codebreaker.GameAPIs.Infrastructure;

namespace Codebreaker.GameAPIs.Services;

public class GamesService(IGamesRepository dataRepository, ILogger<GamesService> logger, GamesMetrics metrics) : IGamesService
{
    public async Task<Game> StartGameAsync(string gameType, string playerName, CancellationToken cancellationToken = default)
    {
        Game game = GamesFactory.CreateGame(gameType, playerName);
        using var activity = Tracing.StartGame(game.Id, game.GameType);

        await dataRepository.AddGameAsync(game, cancellationToken);
        metrics.GameStarted(game);
        return game;
    }

    public async Task<(Game Game, Move Move)> SetMoveAsync(Guid id, string gameType, string[] guesses, int moveNumber, CancellationToken cancellationToken = default)
    {
        Game? game = await dataRepository.GetGameAsync(id, cancellationToken);
        CodebreakerException.ThrowIfNull(game);
        CodebreakerException.ThrowIfEnded(game);
        CodebreakerException.ThrowIfUnexpectedGameType(game, gameType);
        using var activity = Tracing.SetMove(game.Id, game.GameType);

        Move move = game.ApplyMove(guesses, moveNumber);

        // Update the game in the game-service database
        await dataRepository.AddMoveAsync(game, move, cancellationToken);
        metrics.MoveSet(game.Id, DateTime.UtcNow, game.GameType);
        if (game.Ended())
        {
            metrics.GameEnded(game); 
        }
        return (game, move);
    }

    // get the game from the cache or the data repository
    public async ValueTask<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var game = await dataRepository.GetGameAsync(id, cancellationToken);
        return game;
    }

    public async Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await dataRepository.DeleteGameAsync(id, cancellationToken);
    }

    public async Task<Game> EndGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Game? game = await dataRepository.GetGameAsync(id, cancellationToken);
        CodebreakerException.ThrowIfNull(game);
       
        game.EndTime = DateTime.UtcNow;
        TimeSpan duration = game.EndTime.Value - game.StartTime;
        game.Duration = duration;
        metrics.GameEnded(game);
        game = await dataRepository.UpdateGameAsync(game, cancellationToken);
        return game;
    }

    public async Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default)
    {
        return await dataRepository.GetGamesAsync(gamesQuery, cancellationToken);
    }
}
