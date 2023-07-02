using Codebreaker.GameAPIs.Data;

using Microsoft.EntityFrameworkCore;

namespace Codebreaker.Data.Cosmos;

public class GamesCosmosContext : DbContext, IGamesRepository
{
    public GamesCosmosContext(DbContextOptions<GamesCosmosContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer("Gamesv3");
        modelBuilder.ApplyConfiguration(new GameConfiguration());
        modelBuilder.ApplyConfiguration(new GameConfiguration<SimpleGame, ColorField, SimpleColorResult>());
        modelBuilder.ApplyConfiguration(new GameConfiguration<ColorGame, ColorField, ColorResult>());
        modelBuilder.ApplyConfiguration(new GameConfiguration<ShapeGame, ShapeAndColorField, ShapeAndColorResult>());

        modelBuilder.Entity<ColorGame>().OwnsMany(g => g.Moves);
        modelBuilder.Entity<SimpleGame>().OwnsMany(g => g.Moves);
        modelBuilder.Entity<ShapeGame>().OwnsMany(g => g.Moves);
    }

    public DbSet<Game> Games => Set<Game>();

    public async Task AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        Games.Add(game);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task AddMoveAsync(Game game, Move move, CancellationToken cancellationToken = default)
    {
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

    public async Task<IEnumerable<Game>> GetMyGamesAsync(string playerName, CancellationToken cancellationToken = default)
    {
        var games = await Games.Where(g => g.PlayerName == playerName).ToListAsync(cancellationToken);
        return games;
    }

    public async Task<IEnumerable<Game>> GetMyRunningGamesAsync(string playerName, CancellationToken cancellationToken = default)
    {
        var games = await Games.Where(g => g.PlayerName == playerName && g.EndTime == null).ToListAsync(cancellationToken);
        return games;
    }

    public Task<IEnumerable<Game>> GetGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Game>> GetRunningGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
