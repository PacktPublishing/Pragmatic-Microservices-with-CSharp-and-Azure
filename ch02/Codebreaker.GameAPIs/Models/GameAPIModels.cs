using System.Text.Json.Serialization;

namespace Codebreaker.GameAPIs.Models;

[JsonConverter(typeof(JsonStringEnumConverter<GameType>))]
public enum GameType
{
    Game6x4,
    Game6x4Mini,
    Game8x5,
    Game5x5x4
}

public record class CreateGameRequest(GameType GameType, string PlayerName);

public record class CreateGameResponse(Guid GameId, GameType GameType, string PlayerName, int NumberCodes, int MaxMoves)
{
    public required IDictionary<string, IEnumerable<string>> FieldValues { get; init; }
}

public record class UpdateGameRequest(Guid GameId, GameType GameType, string PlayerName, int MoveNumber, bool End = false)
{
    public string[]? GuessPegs { get; set; }
}

public record class UpdateGameResponse(
    Guid GameId,
    GameType GameType,
    int MoveNumber,
    bool Ended,
    bool IsVictory,
    string[]? Results);

public record GameSummary(Guid GameId, string PlayerName, DateTime StartTime, int NumberMoves, bool Won, TimeSpan? Duration);
