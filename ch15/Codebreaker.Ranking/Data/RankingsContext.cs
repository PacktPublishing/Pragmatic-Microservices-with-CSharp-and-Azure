using Codebreaker.GameAPIs.Models;

using Microsoft.EntityFrameworkCore;

namespace Codebreaker.Ranking.Data;

/// <summary>
/// Represents the context for managing game rankings, inheriting from DbContext and implementing IRankingsRepository.
/// </summary>
/// <param name="options">Allows for cancellation of asynchronous operations during database interactions.</param>
public class RankingsContext(DbContextOptions<RankingsContext> options) : DbContext(options), IRankingsRepository
{
    private const string PartitionKey = nameof(PartitionKey);
    private const string ContainerName = "RankingsV3";
    private const string Discriminator = nameof(Discriminator);
    private const string DiscriminatorValue = "RankingV3";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer(ContainerName);

        var gameSummaryModel = modelBuilder.Entity<GameSummary>();
        gameSummaryModel.Property<string>(PartitionKey);
        gameSummaryModel.HasPartitionKey(PartitionKey);
        gameSummaryModel.HasKey(nameof(GameSummary.Id), PartitionKey);

        gameSummaryModel.HasDiscriminator<string>(Discriminator)
            .HasValue(DiscriminatorValue);
    }

    /// <summary>
    /// Provides access to a collection of GameSummary entities. It allows for querying and manipulating game summary
    /// data.
    /// </summary>
    public DbSet<GameSummary> GameSummaries => Set<GameSummary>();

    public static string ComputePartitionKey(GameSummary summary) => 
        summary.StartTime.Date.ToString("yyyyMMdd");

    public void SetPartitionKey(GameSummary summary) =>
        Entry(summary).Property(PartitionKey).CurrentValue =
            ComputePartitionKey(summary);

    /// <summary>
    /// Asynchronously adds a game summary to the collection and saves the changes.
    /// </summary>
    /// <param name="summary">Contains the details of the game summary to be added.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>Returns a task representing the asynchronous operation.</returns>
    public async Task AddGameSummaryAsync(GameSummary summary, CancellationToken cancellationToken = default)
    {
        SetPartitionKey(summary);
        GameSummaries.Add(summary);
        await SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Adds an array of game summaries to a collection and saves the changes asynchronously.
    /// </summary>
    /// <param name="summaries">The array of game summaries to be added to the collection.</param>
    /// <param name="cancellationToken">Used to signal cancellation of the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the game summaries do not all belong to the same day.</exception>
    public async Task AddGameSummariesAsync(GameSummary[] summaries, CancellationToken cancellationToken = default)
    {
        var days = summaries.Select(s => s.StartTime.Date.ToString("yyyyMMdd"));
        if (days.Count() is not 1)
        {
            throw new ArgumentException("All summaries must be for the same day");
        }

        foreach (var summary in summaries)
        {
            SetPartitionKey(summary);
        }

        GameSummaries.AddRange(summaries);
        await SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of game summaries for a specific day, ordered by the number of moves and duration.
    /// </summary>
    /// <param name="day">Specifies the date for which game summaries are to be retrieved.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled before completion if needed.</param>
    /// <returns>A task that represents the asynchronous operation, containing a collection of game summaries.</returns>
    public async Task<IEnumerable<GameSummary>> GetGameSummariesByDayAsync(DateOnly day, CancellationToken cancellationToken = default)
    {
        var partitionKey = day.ToString("yyyyMMdd");
        DateTime from = new(day, TimeOnly.MinValue);
        DateTime to = new(day, TimeOnly.MaxValue);

        // this query needs this composite index that needs to be specified - currently not supported to be specified with EF Core
        // https://github.com/dotnet/efcore/issues/17303
        // add this with the Azure portal 
        //    "compositeIndexes": [
        //    [
        //        {
        //        "path": "/NumberMoves",
        //        "order": "ascending"
        //        },
        //        {
        //        "path": "/Duration",
        //        "order": "ascending"
        //        }
        //    ]
        //]

        return await GameSummaries.WithPartitionKey(partitionKey)
            .Where(s => s.StartTime >= from && s.StartTime <= to)
            .OrderBy(s => s.NumberMoves)
            .ThenBy(s => s.Duration)
            .Take(100)
            .ToListAsync(cancellationToken);
    }
}
