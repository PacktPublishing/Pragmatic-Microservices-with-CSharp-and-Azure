namespace Codebreaker.GameAPIs.Extensions;

public static partial class ApiModelExtensions
{
    public static IList<T> ToPegs<T>(this IEnumerable<string> guesses)
        where T : IParsable<T> =>
        guesses.Select(guess => T.Parse(guess, default)).ToArray();

    public static CreateGameResponse ToCreateGameResponse(this Game game)
    {
        static CreateGameResponse GetColorGameResponse(ColorGame game) => new CreateGameResponse<ColorField>(game.GameId, Enum.Parse<GameType>(game.GameType), game.PlayerName)
        {
            Fields = game.Fields.ToArray()
        };

        static CreateGameResponse GetSimpleGameResponse(SimpleGame game) => new CreateGameResponse<ColorField>(game.GameId, Enum.Parse<GameType>(game.GameType), game.PlayerName)
        {
            Fields = game.Fields.ToArray()
        };

        static CreateGameResponse<ShapeAndColorField> GetShapeGameResponse(ShapeGame game) => new(game.GameId, Enum.Parse<GameType>(game.GameType), game.PlayerName)
        {
            Fields = game.Fields.ToArray()
        };

        return game switch
        {
            ColorGame g => GetColorGameResponse(g),
            SimpleGame g => GetSimpleGameResponse(g),
            ShapeGame g => GetShapeGameResponse(g),
            _ => throw new InvalidOperationException("invalid game type")
        };
    }

    public static SetMoveResponse ToSetMoveResponse(this Game game, string result) =>
        new(game.GameId, Enum.Parse<GameType>(game.GameType), game.LastMoveNumber, result);

    public static GetGamesRankResponse ToGamesRankResponse(this IEnumerable<Game> games, DateOnly date, GameType gameType) =>
        new(date, gameType)
        {
            Games = games.Select(g => new GameInfo(g.GameId, g.PlayerName, g.StartTime, g.Duration ?? TimeSpan.MaxValue)).ToArray()
        };
}
