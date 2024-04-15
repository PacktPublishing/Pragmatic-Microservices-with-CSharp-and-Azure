using Codebreaker.GameAPIs.Models;

using Microsoft.EntityFrameworkCore;

namespace Codebreaker.Ranking.Data;

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

    public DbSet<GameSummary> GameSummaries => Set<GameSummary>();

    public static string ComputePartitionKey(GameSummary summary) => summary.StartTime.Date.ToString("yyyyMMdd");

    public void SetPartitionKey(GameSummary summary) =>
        Entry(summary).Property(PartitionKey).CurrentValue =
            ComputePartitionKey(summary);

    public async Task AddGameSummaryAsync(GameSummary summary, CancellationToken cancellationToken = default)
    {
        SetPartitionKey(summary);
        GameSummaries.Add(summary);
        await SaveChangesAsync(cancellationToken);
    }

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
