using System.Text.Json;
using System.Text.Json.Serialization;

namespace Codebreaker.GameAPIs.Models;

public static class GameExtensions
{
    public static byte[] ToBytes(this Game game) =>
        JsonSerializer.SerializeToUtf8Bytes(game);

    public static Game? ToGame(this byte[] bytes)
    {
        Game? game = JsonSerializer.Deserialize<Game>(bytes);
        return game;
    }

    public static string ToJson(this Game game) =>
        JsonSerializer.Serialize(game);

    public static Game? ToGame(this string json) =>
        JsonSerializer.Deserialize<Game>(json);
}
