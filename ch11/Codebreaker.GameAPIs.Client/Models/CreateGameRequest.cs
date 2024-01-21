

namespace Codebreaker.GameAPIs.Client.Models;

/// <summary>
/// The list of possible game types
/// </summary>
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

/// <summary>
/// Send a CreateGameRequest to start a new game
/// </summary>
/// <param name="GameType">The game type with one of the <see cref="GameType"/>enum values</param>
/// <param name="PlayerName">The name of the player</param>
internal record class CreateGameRequest(
    GameType GameType,
    string PlayerName);
