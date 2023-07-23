using System.Text.Json.Serialization;

namespace Codebreaker.GameAPIs.Client.Models;

[JsonConverter(typeof(JsonStringEnumConverter<GameType>))]
public enum GameType
{
    Game6x4,
    Game6x4Mini,
    Game8x5,
    Game5x5x4,
}

public record class CreateGameRequest(
    GameType GameType,
    string PlayerName);
