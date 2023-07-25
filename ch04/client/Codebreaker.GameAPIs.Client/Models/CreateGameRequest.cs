using System.Text.Json.Serialization;

namespace Codebreaker.GameAPIs.Client.Models;

#if NET8_0_OR_GREATER
[JsonConverter(typeof(JsonStringEnumConverter<GameType>))]
#else
[JsonConverter(typeof(JsonStringEnumConverter))]
#endif
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
