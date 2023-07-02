using Codebreaker.Data.Cosmos.Converters;
using Codebreaker.GameAPIs.Contracts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codebreaker.Data.Cosmos;
internal class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasPartitionKey(g => g.GameId);
        builder.HasKey(g => g.GameId);

        builder.Property(g => g.GameType).HasMaxLength(10);

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<ColorGame>("color")
            .HasValue<ShapeGame>("shape")
            .HasValue<SimpleGame>("simple");
    }
}

internal class GameConfiguration<TGame, TField, TResult> : IEntityTypeConfiguration<TGame>
    where TGame : Game, IGame<TField, TResult>
    where TField : IParsable<TField>
    where TResult : struct
{
    public void Configure(EntityTypeBuilder<TGame> builder)
    {
        builder.HasPartitionKey(g => g.GameId);
        builder.Property(g => g.FieldValues).HasConversion(new FieldValueValueConverter());
    }
}
