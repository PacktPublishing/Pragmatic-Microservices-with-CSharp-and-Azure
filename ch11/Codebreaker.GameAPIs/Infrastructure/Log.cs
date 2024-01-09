namespace Codebreaker.GameAPIs.Extensions;

public static partial class Log
{
    [LoggerMessage(
        EventId = 3000,
        Level = LogLevel.Error,
        Message = "{error}")]
    public static partial void Error(this ILogger logger, Exception ex, string error);

    [LoggerMessage(
               EventId = 3001,
               Level = LogLevel.Warning,
               Message = "Game {gameId} not found")]
    public static partial void GameNotFound(this ILogger logger, Guid gameId);

    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Warning,
        Message = "Invalid move received {gameId}, guesses: {guesses}, {error}")]
    public static partial void InvalidMoveReceived(this ILogger logger, Guid gameId, string guesses, string error);

    [LoggerMessage(
        EventId = 4000,
        Level = LogLevel.Information,
        Message = "The move {move} was set for {game} with result {result}")]
    public static partial void SendMove(this ILogger logger, string move, string game, string result);

    [LoggerMessage(
        EventId = 4001,
        Level = LogLevel.Information,
        Message = "Matched after {count} moves with {game}")]
    public static partial void Matched(this ILogger logger, int count, string game);

}
