using System.Runtime.Serialization;

namespace Codebreaker.GameAPIs.Client.Models;

public enum GameType
{
    [EnumMember(Value = "Game6x4")]
    Game6x4,
    [EnumMember(Value = "Game6x4Mini")]
    Game6x4Mini,
    [EnumMember(Value = "Game8x5")]
    Game8x5,
    [EnumMember(Value = "Game5x5x4")]
    Game5x5x4,
}

public record class CreateGameRequest(
    GameType GameType,
    string PlayerName);
