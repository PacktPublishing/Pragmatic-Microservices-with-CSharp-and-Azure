using System.Text.Json.Serialization;

namespace Codebreaker.GameAPIs.Models;

public enum GameType
{
    Game6x4,
    Game6x4Mini,
    Game8x5,
    Game5x5x4
}

public record class CreateGameRequest(GameType GameType, string PlayerName);

[JsonDerivedType(typeof(CreateGameResponse<ColorField>), "Color")]
[JsonDerivedType(typeof(CreateGameResponse<ShapeAndColorField>), "Shape")]
public record class CreateGameResponse(GameType GameType, string PlayerName)
{
    public Guid GameId { get; init; }
}

public record class CreateGameResponse<TField>(GameType GameType, string PlayerName)
    : CreateGameResponse(GameType, PlayerName)
{
    public required TField[] Fields { get; init; }
}

public record class SetMoveRequest(Guid GameId, GameType GameType, string PlayerName, int MoveNumber)
{
    public required string[] GuessPegs { get; set; }
}

public record SetMoveResponse(
    Guid GameId,
    GameType GameType,
    int MoveNumber,
    string Result);

public record GameInfo(Guid GameId, string PlayerName, DateTime StartTime, TimeSpan Duration);

public record GetGamesRankResponse(DateOnly Date, GameType GameType)
{
    public required IEnumerable<GameInfo> Games { get; set; }
}
