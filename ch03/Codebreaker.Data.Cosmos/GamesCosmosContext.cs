namespace Codebreaker.Data.Cosmos;

public class GamesCosmosContext : DbContext, IGamesRepository
{
    private const string PartitionKey = nameof(PartitionKey);
    private const string ContainerName = "GamesV3";
    private readonly FieldValueValueConverter _fieldValueConverter = new();
    private readonly FieldValueComparer _fieldValueComparer = new();

    public GamesCosmosContext(DbContextOptions<GamesCosmosContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer(ContainerName);
        var gameModel = modelBuilder.Entity<Game>();

        gameModel.Property<string>(PartitionKey);
        gameModel.HasPartitionKey(PartitionKey);
        gameModel.HasKey(nameof(Game.GameId), PartitionKey);

        gameModel.Property(g => g.FieldValues)
            .HasConversion(_fieldValueConverter, _fieldValueComparer);
    }

    public DbSet<Game> Games => Set<Game>();

    public static string ComputePartitionKey(Game game) => game.GameId.ToString();

    public void SetPartitionKey(Game game) =>
        Entry(game).Property(PartitionKey).CurrentValue =
            ComputePartitionKey(game);

    public async Task AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        SetPartitionKey(game);
        Games.Add(game);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task AddMoveAsync(Game game, Move _, CancellationToken cancellationToken = default)
    {
        SetPartitionKey(game);
        Games.Update(game);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var game = await Games
            .WithPartitionKey(gameId.ToString())
            .SingleOrDefaultAsync(g => g.GameId == gameId, cancellationToken);

        if (game is null)
            return false;
        Games.Remove(game);
        await SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<Game?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var game = await Games
            .WithPartitionKey(gameId.ToString())
            .SingleOrDefaultAsync(g => g.GameId == gameId, cancellationToken);
        return game;
    }

    private const int MAXGAMESRETURNED = 500;
    public async Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default)
    {
        IQueryable<Game> query = Games
            .TagWith(nameof(GetGamesAsync));

        // Apply Game filters if provided.
        if (gamesQuery.Date.HasValue)
        {
            DateTime begin = gamesQuery.Date.Value.ToDateTime(TimeOnly.MinValue);
            DateTime end = begin.AddDays(1);
            query = query.Where(g => g.StartTime < end && g.StartTime > begin);
        }
        if (gamesQuery.PlayerName != null)
            query = query.Where(g => g.PlayerName == gamesQuery.PlayerName);
        if (gamesQuery.GameType != null)
            query = query.Where(g => g.GameType == gamesQuery.GameType);
        if (gamesQuery.RunningOnly)
            query = query.Where(g => g.EndTime == null);

        if (gamesQuery.Ended == true)
        {
            query = query.Where(g => g.EndTime != null)
                .OrderBy(g => g.Duration);
        }
        else
        {
            query = query.OrderByDescending(g => g.StartTime);
        }

        query = query.Take(MAXGAMESRETURNED);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Game> UpdateGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        SetPartitionKey(game);
        Games.Add(game);
        await SaveChangesAsync(cancellationToken);
        return game;
    }
}
