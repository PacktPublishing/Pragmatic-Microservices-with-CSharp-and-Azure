using Codebreaker.GameAPIs.Analyzers;
using Codebreaker.GameAPIs.Contracts;

namespace Codebreaker.GameAPIs.Extensions;
public static class GameExtensions
{
    //public static IList<T> ToPegs<T>(this IEnumerable<string> guesses)
    //    where T : IParsable<T> =>
    //    guesses.Select(guess => T.Parse(guess, default)).ToArray();

    //public static string ApplyMove(this Game game, IEnumerable<string> guesses, int moveNumber)
    //{
    //    ColorGameMoveAnalyzer GetColorGameMoveAnalyzer(ColorGame game) =>
    //        new ColorGameMoveAnalyzer(game, guesses.ToPegs<ColorField>(), moveNumber);

    //    SimpleGameMoveAnalyzer GetSimpleGameMoveAnalyzer(SimpleGame game) =>
    //        new SimpleGameMoveAnalyzer(game, guesses.ToPegs<ColorField>(), moveNumber);

    //    ShapeGameMoveAnalyzer GetShapeGameMoveAnalyzer(ShapeGame game) =>
    //        new ShapeGameMoveAnalyzer(game, guesses.ToPegs<ShapeAndColorField>(), moveNumber);

    //    IGameMoveAnalyzer analyzer = game switch
    //    {
    //        ColorGame g => GetColorGameMoveAnalyzer(g),
    //        SimpleGame g => GetSimpleGameMoveAnalyzer(g),
    //        ShapeGame g => GetShapeGameMoveAnalyzer(g),
    //        _ => throw new NotImplementedException()
    //    };

    //    return analyzer.ApplyMove();
    //}
}
