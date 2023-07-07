namespace Codebreaker.GameAPIs.Extensions;

public static partial class ApiModelExtensions
{
    public static IList<T> ToPegs<T>(this IEnumerable<string> guesses)
        where T : IParsable<T> =>
        guesses.Select(guess => T.Parse(guess, default)).ToArray();

    public static CreateGameResponse AsCreateGameResponse(this Game game)
    {       
        return new CreateGameResponse(game.GameId, Enum.Parse<GameType>(game.GameType), game.PlayerName)
        {
            FieldValues = game.FieldValues
        };
    }

    public static SetMoveResponse AsSetMoveResponse(this Game game, string[] result) =>
        new(game.GameId, Enum.Parse<GameType>(game.GameType), game.LastMoveNumber, result);

    public static GetGamesRankResponse ToGamesRankResponse(this IEnumerable<Game> games, DateOnly date, GameType gameType) =>
        new(date, gameType)
        {
            Games = games.Select(g => new GameSummary(
                g.GameId, 
                g.PlayerName, 
                g.StartTime, 
                g.LastMoveNumber,
                g.Won,
                g.Duration)).ToArray()
        };
}
