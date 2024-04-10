namespace Codebreaker.Live.Extensions;

public static partial class Log
{
    [LoggerMessage(
        EventId = 20001,
        Level = LogLevel.Information,
        Message = "Client {client} subscribed to {GameType}")]
    public static partial void ClientSubscribed(this ILogger logger, string client, string gameType);

    [LoggerMessage(
        EventId = 20002,
        Level = LogLevel.Information,
        Message = "Client {client} unsubscribed from {GameType}")]
    public static partial void ClientUnsubscribed(this ILogger logger, string client, string gameType);
}