using Codebreaker.Data.Cosmos.Utilities;
using Codebreaker.GameAPIs.Data;

using Microsoft.EntityFrameworkCore;

namespace Codebreaker.Data.Cosmos;

public class GamesCosmosContext : DbContext, IGamesRepository
{
    public const string PartitionKey = nameof(PartitionKey);
    public const string GameId = nameof(GameId);
    public const string MoveId = nameof(MoveId);
    public const string Id = nameof(Id);
    public const string ETag = nameof(ETag);

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

        //gameModel.Ignore(g => g.GameId);
        //gameModel.Ignore(g => g.Moves);
        // gameModel.HasPartitionKey(PartitionKey);
        // gameModel.Property<string>(PartitionKey);
        // gameModel.Property<string>(ETag).IsETagConcurrency();

        gameModel.Property(g => g.FieldValues)
            .HasConversion(_fieldValueConverter, _fieldValueComparer);

        // modelBuilder.Owned<Move>();

         //gameModel
         //   .OwnsMany(g => g.Moves, moveBuilder =>
         //   {
         //       moveBuilder.ToJsonProperty("Moves");
         //       moveBuilder.Property<string>(PartitionKey);
         //       moveBuilder.Property<Guid>(GameId);
         //       //moveBuilder.Property<Guid>(MoveId).ValueGeneratedOnAdd();
         //       //moveBuilder.WithOwner().HasForeignKey(GameId, PartitionKey);
         //       //moveBuilder.HasKey(MoveId);
         //   });
    }

    public static string ComputePartitionKey(Game game) => game.GameId.ToString();

    public void SetPartitionKey(Game game) =>
        Entry(game).Property(PartitionKey).CurrentValue =
            ComputePartitionKey(game);

    //public void SetMoveId(Move move) =>
    //    Entry(move).Property<Guid>(MoveId).CurrentValue = Guid.NewGuid();

    public DbSet<Game> Games => Set<Game>();

    public async Task AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        // SetPartitionKey(game);
        Games.Add(game);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task AddMoveAsync(Game game, Move move, CancellationToken cancellationToken = default)
    {
        // SetMoveId(move);
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
