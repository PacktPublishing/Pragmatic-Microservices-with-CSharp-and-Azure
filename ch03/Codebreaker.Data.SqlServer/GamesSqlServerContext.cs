using Codebreaker.GameAPIs.Data;

namespace Codebreaker.Data.SqlServer;

public class GamesSqlServerContext(DbContextOptions<GamesSqlServerContext> options) : DbContext(options), IGamesRepository
{
    internal const string GameId = nameof(GameId);
    internal const string MoveId = nameof(MoveId);

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
        var game = await Games.FindAsync(gameId, cancellationToken);
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
}
