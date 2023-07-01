using Codebreaker.GameAPIs.Data;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Codebreaker.Data.SqlServer;

public class GamesSqlServerContext(DbContextOptions<GamesSqlServerContext> options) : DbContext(options), IGamesRepository
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Codebreaker");
        modelBuilder.ApplyConfiguration(new GameConfiguration());

        modelBuilder.ApplyConfiguration(new GameConfiguration<ColorGame, ColorField, ColorResult>("ColorGames"));
        modelBuilder.ApplyConfiguration(new GameConfiguration<SimpleGame, ColorField, SimpleColorResult>("SimpleGames"));
        modelBuilder.ApplyConfiguration(new GameConfiguration<ShapeGame, ShapeAndColorField, ShapeAndColorResult>("ShapeGames"));

        modelBuilder.ApplyConfiguration(new MoveConfiguration());

        modelBuilder.ApplyConfiguration(new MoveConfiguration<ColorMove, ColorField, ColorResult>("ColorMoves"));
        modelBuilder.ApplyConfiguration(new MoveConfiguration<SimpleMove, ColorField, SimpleColorResult>("SimpleMoves"));
        modelBuilder.ApplyConfiguration(new MoveConfiguration<ShapeMove, ShapeAndColorField, ShapeAndColorResult>("ShapeMoves"));

        modelBuilder.Entity<ColorMove>()
            .Property(m => m.KeyPegs)
            .HasColumnType("nvarchar")
            .HasMaxLength(20)
            .HasConversion(
                convertToProviderExpression: peg => peg.ToString(),
                convertFromProviderExpression: peg => ColorResult.Parse(peg!, null),
                valueComparer: new ValueComparer<ColorResult>(favorStructuralComparisons: true));

        modelBuilder.Entity<SimpleMove>()
            .Property(m => m.KeyPegs)
            .HasColumnType("nvarchar")
            .HasMaxLength(40)
            .HasConversion(
                convertToProviderExpression: peg => peg.ToString(),
                convertFromProviderExpression: peg => SimpleColorResult.Parse(peg!, null),
                valueComparer: new ValueComparer<SimpleColorResult>(favorStructuralComparisons: true));

        modelBuilder.Entity<ShapeMove>()
            .Property(m => m.KeyPegs)
            .HasColumnType("nvarchar")
            .HasMaxLength(20)
            .HasConversion(
                convertToProviderExpression: peg => peg.ToString(),
                convertFromProviderExpression: peg => ShapeAndColorResult.Parse(peg!, null),
                valueComparer: new ValueComparer<ShapeAndColorResult>(favorStructuralComparisons: true));

        modelBuilder.Entity<ColorGame>()
            .HasMany(g => g.Moves)
            .WithOne()
            .HasForeignKey(m => m.GameId);

        modelBuilder.Entity<SimpleGame>()
            .HasMany(g => g.Moves)
            .WithOne()
            .HasForeignKey(m => m.GameId);

        modelBuilder.Entity<ShapeGame>()
            .HasMany(g => g.Moves)
            .WithOne()
            .HasForeignKey(m => m.GameId);
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

    public async Task DeleteGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var game = await Games.FindAsync(new object[] { gameId }, cancellationToken);
        if (game is null)
            return;
        Games.Remove(game);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<Game?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var game = await Games.FindAsync(new object[] { gameId }, cancellationToken);
        return game;
    }

    public async Task<IEnumerable<Game>> GetGamesByDateAsync(string gameType, DateOnly date, CancellationToken cancellationToken = default)
    {
        var games = await Games.Where(g => DateOnly.FromDateTime(g.StartTime) == date).ToListAsync(cancellationToken);
        return games;
    }

    public async Task<IEnumerable<Game>> GetGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default)
    {
        var games = await Games.Where(g => g.PlayerName == playerName).ToListAsync(cancellationToken);
        return games;
    }

    public async Task<IEnumerable<Game>> GetRunningGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default)
    {
        var games = await Games.Where(g => g.PlayerName == playerName && g.EndTime == null).ToListAsync(cancellationToken);
        return games;
    }
}
