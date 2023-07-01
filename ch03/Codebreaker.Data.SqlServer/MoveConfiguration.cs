using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Codebreaker.Data.SqlServer;

internal class MoveConfiguration : IEntityTypeConfiguration<Move>
{
    public void Configure(EntityTypeBuilder<Move> builder)
    {
        builder.HasKey(m => m.MoveId);
        builder.UseTpcMappingStrategy();
    }
}

internal class MoveConfiguration<TMove, TField, TResult>(string tableName) : IEntityTypeConfiguration<TMove>
    where TMove : Move<TField, TResult>
    where TField : IParsable<TField>
    where TResult : struct, IParsable<TResult>
{
    public void Configure(EntityTypeBuilder<TMove> builder)
    {
        builder.ToTable(tableName);

        builder.Property(m => m.GuessPegs)
            .HasColumnType("nvarchar")
            .HasMaxLength(140)
            .HasConversion(convertToProviderExpression: pegs => pegs.ToFieldString(),
                           convertFromProviderExpression: pegs => pegs.ToFieldCollection<TField>(),
                           valueComparer: new ValueComparer<ICollection<TField>>(
                               equalsExpression: (a, b) => a!.SequenceEqual(b!),
                               hashCodeExpression: a => a.Aggregate(0, (result, next) => HashCode.Combine(result, next.GetHashCode())),
                               snapshotExpression: a => a.ToList()));
    }
}