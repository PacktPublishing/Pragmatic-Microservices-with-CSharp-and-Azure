namespace Codebreaker.GameAPIs.Client;

internal static partial class Log
{
    [LoggerMessage(2000, LogLevel.Information, "GetGameAsync game {GameId} not found: {ErrorMessage}", EventName = "GetGame")]
    public static partial void GetGameNotFound(this ILogger logger, Guid gameId, string errorMessage);

    [LoggerMessage(5001, LogLevel.Error, "GetGameAsync error {ErrorMessage}", EventName = "GetGameError")]
    public static partial void GetGameError(this ILogger logger, string errorMessage, Exception ex);

    [LoggerMessage(5002, LogLevel.Error, "GetGamesAsync error {ErrorMessage}", EventName ="GetGamesError")]
    public static partial void GetGamesError(this ILogger logger, string errorMessage, Exception ex);

    [LoggerMessage(5003, LogLevel.Error, "StartGameAsync error {ErrorMessage}", EventName ="StartGameError")]
    public static partial void StartGameError(this ILogger logger, string errorMessage, Exception ex);

    [LoggerMessage(5004, LogLevel.Error, "SetMoveAsync error {ErrorMessage}", EventName = "SetMoveError")]
    public static partial void SetMoveError(this ILogger logger, string errorMessage, Exception ex);

    [LoggerMessage(8001, LogLevel.Information, "Game {GameId} created", EventName = "GameCreated")]
    public static partial void GameCreated(this ILogger logger,Guid gameId);

    [LoggerMessage(8002, LogLevel.Information, "Move {MoveNumber} for game {GameId} set", EventName = "MoveSet")]
    public static partial void MoveSet(this ILogger logger, Guid gameId, int moveNumber);

    [LoggerMessage(8003, LogLevel.Information, "Game {GameId} information received, ended {Ended}, number moves {MoveNumber}", EventName = "GameReceived")]
    public static partial void GameReceived(this ILogger logger, Guid gameId, bool ended, int moveNumber);

    [LoggerMessage(8004, LogLevel.Information, "With query {Query}, {NumberGames} games received", EventName = "GamesReceived")]
    public static partial void GamesReceived(this ILogger logger, string query, int numberGames);
}
