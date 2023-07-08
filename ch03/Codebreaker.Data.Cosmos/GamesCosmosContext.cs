using Codebreaker.Data.Cosmos.Utilities;
using Codebreaker.GameAPIs.Data;

using Microsoft.EntityFrameworkCore;

namespace Codebreaker.Data.Cosmos;

public class GamesCosmosContext : DbContext, IGamesRepository
{
    private FieldValueValueConverter _fieldValueConverter = new();
    private FieldValueComparer _fieldValueComparer = new();

    public GamesCosmosContext(DbContextOptions<GamesCosmosContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer("GamesV3");
        modelBuilder.Entity<Game>().HasPartitionKey(g => g.GameId);
        var gameModel = modelBuilder.Entity<Game>();

        gameModel.Property(g => g.FieldValues)
            .HasConversion(_fieldValueConverter, _fieldValueComparer);
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
        var game = await Games.FindAsync(gameId, cancellationToken);
        return game;
    }

    public async Task<IEnumerable<Game>> GetGamesByDateAsync(string gameType, DateOnly date, CancellationToken cancellationToken = default)
    {
        var games = await Games
            .Where(g => DateOnly.FromDateTime(g.StartTime) == date)
            .ToListAsync(cancellationToken);
        return games;
    }

    public async Task<IEnumerable<Game>> GetGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default)
    {
        var games = await Games
            .Where(g => g.PlayerName == playerName)
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
