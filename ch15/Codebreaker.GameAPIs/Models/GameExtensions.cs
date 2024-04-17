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

}
