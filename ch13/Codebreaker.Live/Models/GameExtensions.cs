using Codebreaker.GameAPIs.Extensions;

namespace Codebreaker.Live.Models;

public static class GameExtensions
{
    public static GameSummary ToGameSummary(this Game game) => new(game.Id, game.GameType, game.PlayerName, game.HasEnded(), game.IsVictory, game.StartTime, game.Duration ?? TimeSpan.MaxValue);
}
