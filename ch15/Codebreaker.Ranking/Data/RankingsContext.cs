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

        var gameSummaryModel = modelBuilder.Entity<GameSummary1>();
        gameSummaryModel.Property<string>(PartitionKey);
        gameSummaryModel.HasPartitionKey(PartitionKey);
        gameSummaryModel.HasKey(nameof(GameSummary1.Id), PartitionKey);

        gameSummaryModel.HasDiscriminator<string>(Discriminator)
            .HasValue(DiscriminatorValue);
    }

    public DbSet<GameSummary1> GameSummaries => Set<GameSummary1>();

    public static string ComputePartitionKey(GameSummary1 summary) => summary.StartTime.Date.ToString("yyyyMMdd");

    public void SetPartitionKey(GameSummary1 summary) =>
        Entry(summary).Property(PartitionKey).CurrentValue =
            ComputePartitionKey(summary);

    public async Task AddGameSummaryAsync(GameSummary1 summary, CancellationToken cancellationToken = default)
    {
        SetPartitionKey(summary);
        GameSummaries.Add(summary);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task AddGameSummariesAsync(GameSummary1[] summaries, CancellationToken cancellationToken = default)
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

    public async Task<IEnumerable<GameSummary1>> GetGameSummariesByDayAsync(DateOnly day, CancellationToken cancellationToken = default)
    {
        var partitionKey = day.ToString("yyyyMMdd");
        return await GameSummaries.WithPartitionKey(partitionKey)
            .Where(s => s.StartTime.ToString("yyyyMMdd") == partitionKey)
            .OrderByDescending(s => s.NumberMoves)
            .ThenByDescending(s => s.Duration)
            .Take(100)
            .ToListAsync(cancellationToken);
    }
}
