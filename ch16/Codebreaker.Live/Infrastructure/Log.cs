namespace Codebreaker.Live.Extensions;

public static partial class Log
{
    [LoggerMessage(
        EventId = 20001,
        Level = LogLevel.Information,
        Message = "Client {Client} subscribed to {GameType}")]
    public static partial void ClientSubscribed(this ILogger logger, string client, string gameType);

    [LoggerMessage(
        EventId = 20002,
        Level = LogLevel.Information,
        Message = "Client {Client} unsubscribed from {GameType}")]
    public static partial void ClientUnsubscribed(this ILogger logger, string client, string gameType);

    [LoggerMessage(
        EventId = 20003,
        Level = LogLevel.Information,
        Message = "Client {Client} disconnected")]
    public static partial void ClientDisconnected(this ILogger logger, string client);

    [LoggerMessage(
        EventId = 20004,
        Level = LogLevel.Information,
        Message = "Processing game completion event")]
    public static partial void ProcessingGameCompletionEvent(this ILogger logger);

    [LoggerMessage(
        EventId = 50001,
        Level = LogLevel.Error,
        Message = "Error processing game completion event: {Error}")]
    public static partial void ErrorProcessingGameCompletionEvent(this ILogger logger, Exception ex, string error);
}