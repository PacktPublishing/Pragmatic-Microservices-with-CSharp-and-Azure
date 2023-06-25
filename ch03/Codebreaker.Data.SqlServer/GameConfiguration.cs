using Codebreaker.GameAPIs.Contracts;

namespace Codebreaker.Data.SqlServer;

internal class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(g => g.GameId);

        builder.UseTpcMappingStrategy();

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

    }
}
