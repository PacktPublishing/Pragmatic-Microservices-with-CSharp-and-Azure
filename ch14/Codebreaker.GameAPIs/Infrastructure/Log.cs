namespace Codebreaker.GameAPIs.Extensions;

public static partial class Log
{
    [LoggerMessage(
        EventId = 3000,
        Level = LogLevel.Error,
        Message = "{ErrorMessage}")]
    public static partial void Error(this ILogger logger, Exception ex, string errorMessage);

    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Warning,
        Message = "Game {GameId} not found")]
    public static partial void GameNotFound(this ILogger logger, Guid gameId);

    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Warning,
        Message = "Invalid game type requested: {GameType}")]
    public static partial void InvalidGameType(this ILogger logger, string gameType);

    [LoggerMessage(
        EventId = 3003,
        Level = LogLevel.Warning,
        Message = "Invalid move received {GameId}, guesses: {Guesses}, {ErrorMessage}")]
    public static partial void InvalidMoveReceived(this ILogger logger, Guid gameId, string guesses, string errorMessage);

    [LoggerMessage(
        EventId = 3004,
        Level = LogLevel.Error,
        Message = "Error writing game completed event, game id: {gameId}")]
    public static partial void ErrorWritingGameCompletedEvent(this ILogger logger, Guid gameId, Exception ex);

    [LoggerMessage(
        EventId = 4000,
        Level = LogLevel.Information,
        Message = "The game {GameId} started")]
    public static partial void GameStarted(this ILogger logger, Guid gameId);

    [LoggerMessage(
        EventId = 4001,
        Level = LogLevel.Information,
        Message = "The move {Move} was set for {GameId} with result {Result}")]
    public static partial void SendMove(this ILogger logger, string move, Guid gameId, string result);

    [LoggerMessage(
        EventId = 4002,
        Level = LogLevel.Information,
        Message = "Game won after {Moves} moves and {Seconds} seconds with game {GameId}")]
    private static partial void GameWon(this ILogger logger, int moves, int seconds, Guid gameId);

    [LoggerMessage(
        EventId = 4003,
        Level = LogLevel.Information,
        Message = "Game lost after {Seconds} seconds with game {GameId}")]
    private static partial void GameLost(this ILogger logger, int seconds, Guid gameId);

    public static void GameEnded(this ILogger logger, Game game)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            if (game.IsVictory)
            {
                logger.GameWon(game.Moves.Count, game.Duration?.Seconds ?? 0, game.Id);
            }
            else
            {
                logger.GameLost(game.Duration?.Seconds ?? 0, game.Id);
            }
        }
    }

    [LoggerMessage(
        EventId = 4004,
        Level = LogLevel.Information,
        Message = "Query for game {GameId}")]
    public static partial void QueryGame(this ILogger logger, Guid gameId);

    [LoggerMessage(
        EventId = 4005,
        Level = LogLevel.Information,
        Message = "Returned {NumberGames} games using {Query}")]
    private static partial void QueryGames(this ILogger logger, int numberGames, string query);

    public static void QueryGames(this ILogger logger, IEnumerable<Game> games, string query)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            QueryGames(logger, games.Count(), query);
        }
    }

    [LoggerMessage(
        EventId = 4006,
        Level = LogLevel.Information,
        Message = "Start cleaning up old games")]
    public static partial void StartCleanupGames(this ILogger logger);

    [LoggerMessage(
        EventId = 4007,
        Level = LogLevel.Information,
        Message = "Cleaned up {NumberGames} games")]
    public static partial void CleanedUpGames(this ILogger logger, int numberGames);

    [LoggerMessage(
        EventId = 4008,
        Level = LogLevel.Information,
        Message = "Game start with {GameType} requested")]
    public static partial void GameStart(this ILogger logger, string gameType);
}
