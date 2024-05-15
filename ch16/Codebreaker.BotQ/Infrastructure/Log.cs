namespace CodeBreaker.Bot;

internal static partial class Log
{
    [LoggerMessage(
        EventId = 3000,
        Level = LogLevel.Error,
        Message = "{ErrorMessage}")]
    public static partial void Error(this ILogger logger, Exception ex, string errorMessage);

    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Error,
        Message = "Failed to deserialize JSON message {Message}")]
    public static partial void FailedToDeserializeMessage(this ILogger logger, Exception ex, string message);

    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Error,
        Message = "Failed to decode message {Message}")]
    public static partial void FailedToDecodeMessage(this ILogger logger, Exception ex, string message);

    [LoggerMessage(
        EventId = 3003,
        Level = LogLevel.Warning,
        Message = "Deserialized null message {Message}")]
    public static partial void DeserializedNullMessage(this ILogger logger, string message);

    [LoggerMessage(
        EventId = 3004,
        Level = LogLevel.Warning,
        Message = "Message dequeue count exceeded for message {MessageId} with count {Count}")]
    public static partial void MessageDequeueCountExceeded(this ILogger logger, string messageId, long count);

    [LoggerMessage(
        EventId = 4000,
        Level = LogLevel.Information,
        Message = "Sending the move {Move} to {GameId}")]
    public static partial void SendMove(this ILogger logger, string move, Guid gameId);

    [LoggerMessage(
        EventId = 4001,
        Level = LogLevel.Information,
        Message = "Matched after {NumberMoves} moves with {GameId}")]
    public static partial void Matched(this ILogger logger, int numberMoves, Guid gameId);

    [LoggerMessage(
        EventId = 4002,
        Level = LogLevel.Information,
        Message = "Reduced the possible values to {Number} with {Color} hits in {GameId}")]
    public static partial void ReducedPossibleValues(this ILogger logger, int number, string color, Guid gameId);

    [LoggerMessage(
        EventId = 4003,
        Level = LogLevel.Information,
        Message = "Finished run with {MoveNumber} in {GameId}")]
    public static partial void FinishedRun(this ILogger logger, int moveNumber, Guid gameId);

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
        Message = "Started playing games with sequence {Id}")]
    public static partial void StartedPlayingGames(this ILogger logger, Guid id);
}
