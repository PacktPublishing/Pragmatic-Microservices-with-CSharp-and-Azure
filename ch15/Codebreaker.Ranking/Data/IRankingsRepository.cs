using Codebreaker.GameAPIs.Models;

namespace Codebreaker.Ranking.Data;

/// <summary>
/// Defines methods for managing game summaries, including adding multiple or single summaries and retrieving summaries
/// by date.
/// </summary>
public interface IRankingsRepository
{
    Task AddGameSummariesAsync(GameSummary[] summaries, CancellationToken cancellationToken = default);
    Task AddGameSummaryAsync(GameSummary summary, CancellationToken cancellationToken = default);
    Task<IEnumerable<GameSummary>> GetGameSummariesByDayAsync(DateOnly day, CancellationToken cancellationToken = default);
}