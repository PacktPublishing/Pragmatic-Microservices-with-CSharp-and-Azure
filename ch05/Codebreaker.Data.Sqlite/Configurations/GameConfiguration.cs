using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Codebreaker.Data.Sqlite;

internal class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(g => g.GameId);
      
        builder.Property(g => g.GameType).HasMaxLength(20);
        builder.Property(g => g.PlayerName).HasMaxLength(60);

        builder.Property(g => g.Codes).HasMaxLength(120);

        builder.Property(g => g.FieldValues)
            .HasColumnName("Fields")
            .HasColumnType("nvarchar")
            .HasMaxLength(200)
            .HasConversion(convertToProviderExpression: fields => fields.ToFieldsString(),
                           convertFromProviderExpression: fields => fields.FromFieldsString(),
                           valueComparer: new ValueComparer<IDictionary<string, IEnumerable<string>>>(
                               equalsExpression: (a, b) => a!.SequenceEqual(b!),
                               hashCodeExpression: a => a.Aggregate(0, (result, next) => HashCode.Combine(result, next.GetHashCode())),
                               snapshotExpression: a => a.ToDictionary(kv => kv.Key, kv => kv.Value)));
    }
}
