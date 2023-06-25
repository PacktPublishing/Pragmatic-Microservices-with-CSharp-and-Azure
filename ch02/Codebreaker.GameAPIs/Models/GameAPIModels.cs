using System.Text.Json.Serialization;

using Swashbuckle.AspNetCore.Annotations;

namespace Codebreaker.GameAPIs.Models;

public enum GameType
{
    Game6x4,
    Game6x4Mini,
    Game8x5,
    Game5x5x4
}

public record class CreateGameRequest(GameType GameType, string PlayerName);

[SwaggerDiscriminator("fieldType")]
[SwaggerSubType(typeof(CreateGameResponse<ColorField>), DiscriminatorValue = "color")]
[SwaggerSubType(typeof(CreateGameResponse<ShapeAndColorField>), DiscriminatorValue = "shape")]
[JsonDerivedType(typeof(CreateGameResponse<ColorField>), "Color")]
[JsonDerivedType(typeof(CreateGameResponse<ShapeAndColorField>), "Shape")]
public record class CreateGameResponse(Guid GameId, GameType GameType, string PlayerName)
{
}

public record class CreateGameResponse<TField>(Guid GameId, GameType GameType, string PlayerName)
    : CreateGameResponse(GameId, GameType, PlayerName)
{
    public required ILookup<string, string> FieldValues { get; init; }
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
