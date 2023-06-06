using Codebreaker.GameAPIs.Contracts;

namespace Codebreaker.GameAPIs.Models;

public static partial class ApiModelExtensions
{
    public static IList<T> ToPegs<T>(this IEnumerable<string> guesses)
        where T : IParsable<T> =>
        guesses.Select(guess => T.Parse(guess, default)).ToArray();

    public static CreateGameResponse ToCreateGameResponse(this Game game)
    {
        static CreateGameResponse GetColorGameResponse(ColorGame game) => new CreateGameResponse<ColorField>( game.GameId, Enum.Parse<GameType>(game.GameType), game.PlayerName)
        {
            Fields = game.Fields.ToArray()
        };

        static CreateGameResponse GetSimpleGameResponse(SimpleGame game) => new CreateGameResponse<ColorField>( game.GameId, Enum.Parse<GameType>(game.GameType), game.PlayerName)
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

    public static SetMoveResponse ToSetMoveResponse(this Game game)
    {
        static SetMoveResponse GetSetMoveResponse<TField, TResult>(IGame<TField, TResult> game)
            where TResult : struct =>
            new SetMoveResponse(
                game.GameId,
                Enum.Parse<GameType>(game.GameType),
                game.LastMoveNumber,
                game.Moves.Last().KeyPegs?.ToString() ?? string.Empty);

        return game switch
        {
            ColorGame g => GetSetMoveResponse(g),
            SimpleGame g => GetSetMoveResponse(g),
            ShapeGame g => GetSetMoveResponse(g),
            _ => throw new InvalidOperationException("invalid game type")
        };
    }

    public static GetGamesRankResponse ToGamesRankResponse(this IEnumerable<Game> games, DateOnly date, GameType gameType)
    {
        return new GetGamesRankResponse(date, gameType)
        {
            Games = games.Select(g => new GameInfo(g.GameId, g.PlayerName, g.StartTime, g.Duration ?? TimeSpan.MaxValue)).ToArray()
        };
    }
}
