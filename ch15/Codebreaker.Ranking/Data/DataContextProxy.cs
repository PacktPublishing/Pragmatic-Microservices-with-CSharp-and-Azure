using Codebreaker.GameAPIs.Models;

using Microsoft.EntityFrameworkCore;

namespace Codebreaker.Ranking.Data;

/// <summary>
/// Acts as a proxy for database operations related to games, implementing the IGamesRepository interface.
/// </summary>
/// <typeparam name="TContext">Represents a specific type of database context that supports game-related operations.</typeparam>
/// <param name="context">Provides the instance of the database context used to perform operations on game data.</param>
internal class DataContextProxy<TContext>(TContext context) : IRankingsRepository
    where TContext : DbContext, IRankingsRepository
{
    public Task AddGameSummariesAsync(GameSummary[] summaries, CancellationToken cancellationToken = default) => 
        context.AddGameSummariesAsync(summaries, cancellationToken);

    public Task AddGameSummaryAsync(GameSummary summary, CancellationToken cancellationToken = default) => 
        context.AddGameSummaryAsync(summary, cancellationToken);

    public Task<IEnumerable<GameSummary>> GetGameSummariesByDayAsync(DateOnly day, CancellationToken cancellationToken = default) => 
        context.GetGameSummariesByDayAsync(day, cancellationToken);
}
