namespace Codebreaker.Data.SqlServer;

internal class MoveConfiguration : IEntityTypeConfiguration<Move>
{
    public void Configure(EntityTypeBuilder<Move> builder)
    {
        // shadow properties for id and foreign key
        builder.Property<Guid>(GamesSqlServerContext.MoveId);
        builder.Property<Guid>(GamesSqlServerContext.GameId);

        builder.HasKey(GamesSqlServerContext.MoveId);

        builder.Property(g => g.GuessPegs).HasMaxLength(120);
        builder.Property(g => g.KeyPegs).HasMaxLength(60);
    }
}
