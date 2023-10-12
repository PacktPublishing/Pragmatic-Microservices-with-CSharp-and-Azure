using Codebreaker.GameAPIs.Data;

namespace Codebreaker.Data.SqlServer;

public class GamesSqlServerContext(DbContextOptions<GamesSqlServerContext> options) : DbContext(options), IGamesRepository
{
    internal const string GameId = nameof(GameId);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("codebreaker");
        modelBuilder.ApplyConfiguration(new GameConfiguration());
        modelBuilder.ApplyConfiguration(new MoveConfiguration());

        modelBuilder.Entity<Game>()
            .HasMany(g => g.Moves)
            .WithOne()
            .HasForeignKey(GameId);
    }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Move> Moves => Set<Move>();

    public async Task AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        Games.Add(game);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task AddMoveAsync(Game game, Move move, CancellationToken cancellationToken = default)
    {
        Moves.Add(move);
        Games.Update(game);
       
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var game = await Games.FindAsync([gameId], cancellationToken: cancellationToken);
        if (game is null)
            return false;
        Games.Remove(game);
        await SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<Game?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var game = await Games
            .Include("Moves")
            .TagWith(nameof(GetGameAsync))
            .SingleOrDefaultAsync(g => g.GameId == gameId, cancellationToken);
        return game;
    }

    public async Task<IEnumerable<Game>> GetGamesByDateAsync(string gameType, DateOnly date, CancellationToken cancellationToken = default)
    {
        var d = date.ToDateTime(TimeOnly.MinValue);
        var games = await Games
            .Where(g => g.GameType == gameType && g.StartTime.Date == d)
            .TagWith(nameof(GetGamesByDateAsync))
            .ToListAsync(cancellationToken);
                return games;
    }

    public async Task<IEnumerable<Game>> GetGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default)
    {
        var games = await Games
            .Where(g => g.PlayerName == playerName)
            .TagWith(nameof(GetGamesByPlayerAsync))
            .ToListAsync(cancellationToken);
        return games;
    }

    public async Task<IEnumerable<Game>> GetRunningGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default)
    {
        var games = await Games
            .Where(g => g.PlayerName == playerName && g.EndTime == null)
            .ToListAsync(cancellationToken);
        return games;
    }

    private const int MaxGamesReturned = 500;

    public async Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default)
    {
        IQueryable<Game> query = Games
            .TagWith(nameof(GetGamesAsync))
            .Include(g => g.Moves);

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

        if (gamesQuery.Ended)
        {
            query = query.Where(g => g.EndTime != null)
                .OrderBy(g => g.Duration);
        }
        else
        {
            query = query.OrderByDescending(g => g.StartTime);
        }

        query = query.Take(MaxGamesReturned);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Game> UpdateGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        Games.Update(game);
        await SaveChangesAsync(cancellationToken);
        return game;
    }
}
