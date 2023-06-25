using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text.Json;

using Codebreaker.GameAPIs.Data;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        // TODO: change to use generic implementation
        // TODO: set useful maxlength values
        // TODO: add value comparers

        modelBuilder.Entity<ColorGame>().Property(b => b.Codes)
            .HasColumnName("Codes")
            .HasColumnType("nvarchar")
            .HasMaxLength(150)
            .HasConversion(
                codes => codes.ToFieldString(),
                        codes => codes.ToFieldCollection<ColorField>());

        modelBuilder.Entity<SimpleGame>().Property(b => b.Codes)
            .HasColumnName("Codes")
            .HasColumnType("nvarchar")
            .HasMaxLength(150)
            .HasConversion(
                codes => codes.ToFieldString(),
                        codes => codes.ToFieldCollection<ColorField>());

        modelBuilder.Entity<ShapeGame>().Property(b => b.Codes)
            .HasColumnName("Codes")
            .HasColumnType("nvarchar")
            .HasMaxLength(150)
            .HasConversion(
                codes => codes.ToFieldString(),
                        codes => codes.ToFieldCollection<ShapeAndColorField>());

        modelBuilder.Entity<ColorGame>().Property(g => g.FieldValues)
            .HasColumnName("Fields")
            .HasColumnType("nvarchar")
            .HasMaxLength(150)
            .HasConversion(
                convertToProviderExpression: fields => fields.ToLookupString(),
                convertFromProviderExpression: fields => fields.ToLookup());

        modelBuilder.Entity<SimpleGame>().Property(g => g.FieldValues)
            .HasColumnName("Fields")
            .HasColumnType("nvarchar")
            .HasMaxLength(150)
            .HasConversion(
                convertToProviderExpression: fields => fields.ToLookupString(),
                convertFromProviderExpression: fields => fields.ToLookup());

        modelBuilder.Entity<ShapeGame>().Property(g => g.FieldValues)
            .HasColumnName("Fields")
            .HasColumnType("nvarchar")
            .HasMaxLength(150)
            .HasConversion(
                convertToProviderExpression: fields => fields.ToLookupString(),
                convertFromProviderExpression: fields => fields.ToLookup());

        modelBuilder.ApplyConfiguration(new MoveConfiguration());

        modelBuilder.Entity<ColorMove>().ToTable("ColorMoves");
        modelBuilder.Entity<SimpleMove>().ToTable("SimpleMoves");
        modelBuilder.Entity<ShapeMove>().ToTable("ShapeMoves");

        modelBuilder.Entity<ColorMove>()
            .Property(m => m.KeyPegs)
            .HasColumnType("nvarchar")
            .HasMaxLength(20)
            .HasConversion(
                peg => peg.ToString(),
                peg => ColorResult.Parse(peg!, null));

        modelBuilder.Entity<SimpleMove>()
            .Property(m => m.KeyPegs)
            .HasColumnType("nvarchar")
            .HasMaxLength(20)
            .HasConversion(
                peg => peg.ToString(),
                peg => SimpleColorResult.Parse(peg!, null));

        modelBuilder.Entity<ShapeMove>()
            .Property(m => m.KeyPegs)
            .HasColumnType("nvarchar")
            .HasMaxLength(20)
            .HasConversion(
                peg => peg.ToString(),
                peg => ShapeAndColorResult.Parse(peg!, null));

        modelBuilder.Entity<ColorMove>()
            .Property(m => m.GuessPegs)
            .HasColumnType("nvarchar")
            .HasMaxLength(30)
            .HasConversion(
                pegs => pegs.ToFieldString(),
                pegs => pegs.ToFieldCollection<ColorField>());


        modelBuilder.Entity<SimpleMove>()
            .Property(m => m.GuessPegs)
            .HasColumnType("nvarchar")
            .HasMaxLength(30)
            .HasConversion(
                pegs => pegs.ToFieldString(),
                pegs => pegs.ToFieldCollection<ColorField>());


        modelBuilder.Entity<ShapeMove>()
            .Property(m => m.GuessPegs)
            .HasColumnType("nvarchar")
            .HasMaxLength(30)
            .HasConversion(
                pegs => pegs.ToFieldString(),
                pegs => pegs.ToFieldCollection<ShapeAndColorField>());

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

    public async Task UpdateGameAsync(Game game, CancellationToken cancellationToken = default)
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

public class ColorResultValueConverter : ValueConverter<ColorResult, int>
{
    public ColorResultValueConverter(ConverterMappingHints? mappingHints = null) : 
        base((cr => cr.Correct << 4 + cr.WrongPosition), (n => new ColorResult(n >> 4, n & 0b1111)))
    {
    }
}

public class ColorResultConverter : ValueConverter<ColorResult, string>
{
    public ColorResultConverter() :
        base(cr => MapColorResultToString(cr), s => MapStringToColorResult(s), null)
    {

    }

    public ColorResultConverter(ConverterMappingHints? mappingHints = default) : 
        base(cr => MapColorResultToString(cr), s => MapStringToColorResult(s), mappingHints)
    {
    }

    private static string MapColorResultToString(ColorResult r) => r.ToString();
    private static ColorResult MapStringToColorResult(string s) => Enum.Parse<ColorResult>(s);
}

public class ParsableValueConverter<TModel> : ValueConverter<TModel, string>
    where TModel : IParsable<TModel>
{
    public ParsableValueConverter() : 
        base(convertToProviderExpression: t => MapTToString(t), convertFromProviderExpression: s => MapStringToT(s))
        {}

    private static string MapTToString(TModel value) => value.ToString() ?? throw new InvalidOperationException();
    private static TModel MapStringToT(string s) => TModel.Parse(s, default);
}

public class ColorFieldCollectionConverter : ValueConverter<IEnumerable<ColorField>, string>
{
    public ColorFieldCollectionConverter(Expression<Func<IEnumerable<ColorField>, string>> convertToProviderExpression, Expression<Func<string, IEnumerable<ColorField>>> convertFromProviderExpression, ConverterMappingHints? mappingHints = null) :
        base(coll => ColorFieldCollectionToString(coll), s => StringToColorFieldCollection(s), mappingHints)
    {
    }

    private static string ColorFieldCollectionToString(IEnumerable<ColorField> coll) =>
        string.Join("#", coll.Select(cf => cf.ToString()));

    private static IEnumerable<ColorField> StringToColorFieldCollection(string result) =>
        result.Split("#").Select(s => ColorField.Parse(s)).ToArray();
}
