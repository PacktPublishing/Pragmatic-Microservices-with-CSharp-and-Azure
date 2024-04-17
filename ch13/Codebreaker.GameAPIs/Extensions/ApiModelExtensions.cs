namespace Codebreaker.GameAPIs.Extensions;

public static partial class ApiModelExtensions
{
    public static IList<T> ToPegs<T>(this IEnumerable<string> guesses)
        where T : IParsable<T> =>
        guesses.Select(guess => T.Parse(guess, default)).ToArray();

    public static CreateGameResponse ToCreateGameResponse(this Game game) => 
        new(game.Id, Enum.Parse<GameType>(game.GameType), game.PlayerName, game.NumberCodes, game.MaxMoves)
        {
            FieldValues = game.FieldValues
        };

    public static UpdateGameResponse ToUpdateGameResponse(this Game game, string[] result) =>
        new(game.Id, Enum.Parse<GameType>(game.GameType), game.LastMoveNumber, game.HasEnded(), game.IsVictory, result);

    public static UpdateGameResponse ToUpdateGameResponse(this Game game) =>
        new(game.Id, Enum.Parse<GameType>(game.GameType), game.LastMoveNumber, game.HasEnded(), game.IsVictory, null);
}
