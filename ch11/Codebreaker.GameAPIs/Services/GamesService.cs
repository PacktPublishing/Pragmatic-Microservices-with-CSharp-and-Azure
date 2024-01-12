namespace Codebreaker.GameAPIs.Services;

public class GamesService(IGamesRepository dataRepository, ILogger<GamesService> logger, GamesMetrics metrics) : IGamesService
{
    public async Task<Game> StartGameAsync(string gameType, string playerName, CancellationToken cancellationToken = default)
    {
        Game game;
        try
        {
            game = GamesFactory.CreateGame(gameType, playerName);
            using var activity = GamesActivity.StartGame(game.Id, game.GameType);

            await dataRepository.AddGameAsync(game, cancellationToken);
            metrics.GameStarted(game);
            logger.GameStarted(game.Id);
        }
        catch (CodebreakerException ex) when (ex.Code is CodebreakerExceptionCodes.InvalidGameType)
        {
            logger.InvalidGameType(gameType);
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, ex.Message);
            throw;
        }
        return game;
    }

    public async Task<(Game Game, Move Move)> SetMoveAsync(Guid id, string gameType, string[] guesses, int moveNumber, CancellationToken cancellationToken = default)
    {
        Game? game = default;
        Move? move;
        try
        {
            game = await dataRepository.GetGameAsync(id, cancellationToken);
            CodebreakerException.ThrowIfNull(game);
            CodebreakerException.ThrowIfEnded(game);
            CodebreakerException.ThrowIfUnexpectedGameType(game, gameType);
            using var activity = GamesActivity.SetMove(game.Id, game.GameType);

            move = game.ApplyMove(guesses, moveNumber);

            // Update the game in the game-service database
            await dataRepository.AddMoveAsync(game, move, cancellationToken);
            metrics.MoveSet(game.Id, DateTime.UtcNow, game.GameType);
            if (game.Ended())
            {
                logger.GameEnded(game);
                metrics.GameEnded(game);
            }
        }
        catch (ArgumentException ex)
        {
            logger.InvalidMoveReceived(game?.Id ?? Guid.Empty, string.Join(':', guesses), ex.Message);
            metrics.InvalidMove();
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, ex.Message);
            throw;
        }
        return (game, move);
    }

    // get the game from the cache or the data repository
    public async ValueTask<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var game = await dataRepository.GetGameAsync(id, cancellationToken);
        if (game is null)
        {
            logger.GameNotFound(id);
        }
        else
        {
            logger.QueryGame(game.Id);
        }
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
        var games = await dataRepository.GetGamesAsync(gamesQuery, cancellationToken);
        logger.QueryGames(games, gamesQuery.ToString());
        return games;
    }
}
