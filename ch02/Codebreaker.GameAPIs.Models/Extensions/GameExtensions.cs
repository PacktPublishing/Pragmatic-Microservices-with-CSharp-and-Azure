namespace Codebreaker.GameAPIs.Extensions;

public static class GameExtensions
{
    [Obsolete("Use HasEnded instead")]
    public static bool Ended(this Game game) => HasEnded(game);
    public static bool HasEnded(this Game game) => game.EndTime != null;
}
