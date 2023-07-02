using Codebreaker.GameAPIs.Contracts;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Codebreaker.Data.SqlServer;

internal class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(g => g.GameId);

        builder.UseTpcMappingStrategy();
        builder.Property(g => g.GameType).HasMaxLength(20);
        builder.Property(g => g.PlayerName).HasMaxLength(60);
    }
}

internal class GameConfiguration<TGame, TField, TResult>(string tableName) : IEntityTypeConfiguration<TGame>
    where TGame : Game, IGame<TField, TResult>
    where TField : IParsable<TField>
    where TResult : struct
{
    private readonly string _tableName = tableName;

    public void Configure(EntityTypeBuilder<TGame> builder)
    {
        builder.ToTable(_tableName);

        builder.Property(b => b.Codes)
            .HasColumnName("Codes")
            .HasColumnType("nvarchar")
            .HasMaxLength(140)
            .HasConversion(convertToProviderExpression: codes => codes.ToFieldString(),
                           convertFromProviderExpression: codes => codes.ToFieldCollection<TField>(),
                           valueComparer: new ValueComparer<IEnumerable<TField>>(
                               equalsExpression: (a, b) => a!.SequenceEqual(b!),
                               hashCodeExpression: a => a.Aggregate(0, (result, next) => HashCode.Combine(result, next.GetHashCode())),
                               snapshotExpression: a => a.ToList()));

        builder.Property(g => g.FieldValues)
            .HasColumnName("Fields")
            .HasColumnType("nvarchar")
            .HasMaxLength(200)
            .HasConversion(convertToProviderExpression: fields => fields.ToFieldsString(),
                           convertFromProviderExpression: fields => fields.ToFieldsDictionary(),
                           valueComparer: new ValueComparer<IDictionary<string, IEnumerable<string>>>(
                               equalsExpression: (a, b) => a!.SequenceEqual(b!),
                               hashCodeExpression: a => a.Aggregate(0, (result, next) => HashCode.Combine(result, next.GetHashCode())),
                               snapshotExpression: a => a.ToDictionary(kv => kv.Key, kv => kv.Value)));
    }
}
