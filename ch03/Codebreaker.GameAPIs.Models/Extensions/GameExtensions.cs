namespace Codebreaker.GameAPIs.Extensions;

public static class GameExtensions
{
    public static bool HasEnded(this Game game) => game.EndTime != null;
}
