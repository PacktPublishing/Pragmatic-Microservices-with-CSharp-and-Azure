using System.Text.Json;

namespace Codebreaker.GameAPIs.Models;

public static class GameExtensions
{
    public static byte[] ToBytes(this Game game) =>
        JsonSerializer.SerializeToUtf8Bytes(game);

    public static Game? ToGame(this byte[] bytes) =>
        JsonSerializer.Deserialize<Game>(bytes);

    public static string ToJson(this Game game) =>
        JsonSerializer.Serialize(game);

    public static Game? ToGame(this string json) =>
        JsonSerializer.Deserialize<Game>(json);

    // Todo: move to backend models
    public static GameSummary ToGameSummary(this Game game) => new(game.Id, game.GameType, game.PlayerName, game.HasEnded(), game.IsVictory, game.StartTime, game.Duration ?? TimeSpan.MaxValue);

    public static GameSummary1 ToGameSummary1(this Game game) => new GameSummary1
    {
        Id = game.Id,
        GameType = game.GameType,
        PlayerName = game.PlayerName,
        IsCompleted = game.HasEnded(),
        IsVictory = game.IsVictory,
        StartTime = game.StartTime,
        Duration = game.Duration ?? TimeSpan.MaxValue
    };
}
