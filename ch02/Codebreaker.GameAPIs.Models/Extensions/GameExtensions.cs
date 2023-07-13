namespace Codebreaker.GameAPIs.Extensions;

public static class GameExtensions
{
    public static bool Ended(this Game game) => game.EndTime != null;
}
