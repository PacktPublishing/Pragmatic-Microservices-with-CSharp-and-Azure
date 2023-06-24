using Codebreaker.GameAPIs.Analyzers;
using Codebreaker.GameAPIs.Contracts;
using Codebreaker.GameAPIs.Exceptions;

namespace Codebreaker.GameAPIs.Services;

public static class GamesFactory
{
    private static readonly string[] s_colors6 = { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple, Colors.Orange };
    private static readonly string[] s_colors8 = { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple, Colors.Orange, Colors.Pink, Colors.Brown };
    private static readonly string[] s_colors5 = { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple };
    private static readonly string[] s_shapes5 = { Shapes.Circle, Shapes.Square, Shapes.Triangle, Shapes.Star, Shapes.Rectangle };

    public static Game CreateGame(string gameType, string playerName, bool isAuthenticated)
    {
        SimpleGame Create6x4SimpleGame() =>
            new(Guid.NewGuid(), gameType, playerName,  DateTime.Now, 4, 12)
            {
                FieldValues = s_colors6.ToLookup(_ => "Colors"),
                Codes = Random.Shared.GetItems(s_colors6, 4).Select(c => new ColorField(c)).ToArray()
            };

        ColorGame Create6x4Game() =>
            new(Guid.NewGuid(), gameType, playerName, DateTime.Now, 4, 12)
            {
                FieldValues = s_colors6.ToLookup(_ => "Colors"),
                Codes = Random.Shared.GetItems(s_colors6, 4).Select(c => new ColorField(c)).ToArray()
            };

        ColorGame Create8x5Game() =>
            new(Guid.NewGuid(), gameType, playerName, DateTime.Now, 5, 12)
            {
                FieldValues = s_colors8.ToLookup(_ => "Colors"),
                Codes = Random.Shared.GetItems(s_colors8, 5).Select(c => new ColorField(c)).ToArray()
            };

        ShapeGame Create5x5x4Game() =>
            new(Guid.NewGuid(), gameType, playerName, DateTime.Now, 4, 14)
            {
                FieldValues = new List<string[]>
                {
                    s_colors5,
                    s_shapes5
                }.SelectMany(c => c.Select(
                    s => (key: c == s_colors5 ? "Colors" : "Shapes", value: s)))
                .ToLookup(keySelector: c => c.key, elementSelector: c => c.value),
                Codes = Random.Shared.GetItems(s_shapes5, 4)
                    .Zip(Random.Shared.GetItems(s_colors5, 4), (s, c) => new ShapeAndColorField(s, c)).ToArray()
            };

        return gameType switch
        {
            GameTypes.Game6x4Mini => Create6x4SimpleGame(),
            GameTypes.Game6x4 => Create6x4Game(),
            GameTypes.Game8x5 => Create8x5Game(),
            GameTypes.Game5x5x4 => Create5x5x4Game(),
            _ => throw new InvalidGameException("Invalid game type") { HResult = 4000 }
        };
    }

    public static IGameGuessAnalyzer CreateAnalyzer<TField, TResult>(this Game game, IList<TField> guesses, int moveNumber)
        where TField: IParsable<TField>
        where TResult: struct, IParsable<TResult>
    {
        return (game, guesses) switch
        {
            (ColorGame g, IList<ColorField> gu)   => new ColorGameGuessAnalyzer(g, gu, moveNumber),
            (SimpleGame g, IList<ColorField> gu)  => new SimpleGameGuessAnalyzer(g, gu, moveNumber),
            (ShapeGame g, IList<ShapeAndColorField> gu) => new ShapeGameGuessAnalyzer(g, gu, moveNumber),
            _ => throw new NotSupportedException()
        };       
    }

    private static string ApplyMoveCore<TField, TResult>(Game<TField, TResult> game, IList<TField> guesses, int moveNumber)
        where TField: IParsable<TField>
        where TResult: struct, IParsable<TResult>
    {
        static TResult ToResult(string result) => 
            TResult.Parse(result, null);

        IGameGuessAnalyzer analyzer = (game, guesses) switch
        {
            (ColorGame g, IList<ColorField> gu) => new ColorGameGuessAnalyzer(g, gu, moveNumber),
            (SimpleGame g, IList<ColorField> gu) => new SimpleGameGuessAnalyzer(g, gu, moveNumber),
            (ShapeGame g, IList<ShapeAndColorField> gu) => new ShapeGameGuessAnalyzer(g, gu, moveNumber),
            _ => throw new NotSupportedException()
        };
        string result = analyzer.GetResult();
        game.AddMove(guesses.ToArray(), ToResult(result), moveNumber);
        return result;
    }

    public static string ApplyMove(this Game game, IEnumerable<string> guesses, int moveNumber)
    {
        static IList<TField> GetGuesses<TField>(IEnumerable<string> guesses)
            where TField: IParsable<TField>
        {
            return guesses.Select(g => TField.Parse(g, default)).ToList();
        }

        string result = game switch
        {
            ColorGame g => ApplyMoveCore(g, GetGuesses<ColorField>(guesses), moveNumber),
            SimpleGame g => ApplyMoveCore(g, GetGuesses<ColorField>(guesses), moveNumber),
            ShapeGame g => ApplyMoveCore(g, GetGuesses<ShapeAndColorField>(guesses), moveNumber),
            _ => throw new NotSupportedException()
        };
        return result;
    }
}
