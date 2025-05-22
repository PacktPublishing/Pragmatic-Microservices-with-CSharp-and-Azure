namespace CodeBreaker.Bot;

public static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 3000,
        Level = LogLevel.Error,
        Message = "{Message}")]
    public static partial void Error(this ILogger logger, Exception ex, string message);

    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Error,
        Message = "Error starting game {GameId}")]
    public static partial void ErrorStartingGame(this ILogger logger, Exception ex, Guid gameId);

    [LoggerMessage(
        EventId = 4000,
        Level = LogLevel.Information,
        Message = "Sending the move {Move} to {Game}")]
    public static partial void SendMove(this ILogger logger, string move, string game);

    [LoggerMessage(
        EventId = 4001,
        Level = LogLevel.Information,
        Message = "Matched after {Count} moves with {Game}")]
    public static partial void Matched(this ILogger logger, int count, string game);

    [LoggerMessage(
        EventId = 4002,
        Level = LogLevel.Information,
        Message = "Reduced the possible values to {Number} with {Color} hits in {Game}")]
    public static partial void ReducedPossibleValues(this ILogger logger, int number, string color, string game);

    [LoggerMessage(
        EventId = 4003,
        Level = LogLevel.Information,
        Message = "Finished run with {Number} in {Game}")]
    public static partial void FinishedRun(this ILogger logger, int number, string game);

    [LoggerMessage(
        EventId = 4004,
        Level = LogLevel.Information,
        Message = "Using URI {Uri} to access the API service")]
    public static partial void UsingUri(this ILogger logger, string uri);

    [LoggerMessage(
        EventId = 4005,
        Level = LogLevel.Information,
        Message = "Start CodeBreakerGameRunner")]
    public static partial void StartGameRunner(this ILogger logger);

    [LoggerMessage
        (EventId = 4006,
        Level = LogLevel.Trace,
        Message = "Waiting for next timer tick in loop {Loop}")]
    public static partial void WaitingForNextTick(this ILogger logger, int loop);

    [LoggerMessage
        (EventId = 4007,
        Level = LogLevel.Trace,
        Message = "Timer tick fired in loop {Loop}")]
    public static partial void TimerTickFired(this ILogger logger, int loop);

    [LoggerMessage
        (EventId = 4008,
        Level = LogLevel.Information,
        Message = "GameRunner stopped")]
    public static partial void GameRunnerStopped(this ILogger logger);

    [LoggerMessage
        (EventId = 4009,
        Level = LogLevel.Warning,
        Message = "Timer tick returned false, possibly due to cancellation or disposal at loop {Loop}")]
    public static partial void TimerTickFailed(this ILogger logger, int loop);
}
