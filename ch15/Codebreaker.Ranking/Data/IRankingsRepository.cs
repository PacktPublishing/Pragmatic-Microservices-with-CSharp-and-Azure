using Codebreaker.GameAPIs.Models;

namespace Codebreaker.Ranking.Data;

public interface IRankingsRepository
{
    Task AddGameSummariesAsync(GameSummary[] summaries, CancellationToken cancellationToken = default);
    Task AddGameSummaryAsync(GameSummary summary, CancellationToken cancellationToken = default);
    Task<IEnumerable<GameSummary>> GetGameSummariesByDayAsync(DateOnly day, CancellationToken cancellationToken = default);
}