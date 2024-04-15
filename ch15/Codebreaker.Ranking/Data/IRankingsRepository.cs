using Codebreaker.GameAPIs.Models;

namespace Codebreaker.Ranking.Data;

public interface IRankingsRepository
{
    Task AddGameSummariesAsync(GameSummary1[] summaries, CancellationToken cancellationToken = default);
    Task AddGameSummaryAsync(GameSummary1 summary, CancellationToken cancellationToken = default);
    Task<IEnumerable<GameSummary1>> GetGameSummariesByDayAsync(DateOnly day, CancellationToken cancellationToken = default);
}