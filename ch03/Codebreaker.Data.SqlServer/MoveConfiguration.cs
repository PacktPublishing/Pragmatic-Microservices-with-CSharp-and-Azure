namespace Codebreaker.Data.SqlServer;

internal class MoveConfiguration : IEntityTypeConfiguration<Move>
{
    public void Configure(EntityTypeBuilder<Move> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(m => m.MoveId);

        //builder.HasDiscriminator<string>("MoveType")
        //    .HasValue<ColorMove>("color")
        //    .HasValue<ShapeMove>("shape")
        //    .HasValue<SimpleMove>("simple");
    }
}