namespace Codebreaker.Data.Sqlite;

internal class MoveConfiguration : IEntityTypeConfiguration<Move>
{
    public void Configure(EntityTypeBuilder<Move> builder)
    {
        // shadow property for the foreign key
        builder.Property<Guid>(GamesSqliteContext.GameId);

        builder.Property(g => g.GuessPegs).HasMaxLength(120);
        builder.Property(g => g.KeyPegs).HasMaxLength(60);
    }
}
