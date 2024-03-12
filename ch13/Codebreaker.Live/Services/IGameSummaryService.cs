namespace Codebreaker.Live.Services;

public interface IGameSummaryService
{
    Task AddGameAsync(GameSummary game);
    //IAsyncEnumerable<GameSummary> GetGamesAsync(string gameType, CancellationToken token = default);
}
