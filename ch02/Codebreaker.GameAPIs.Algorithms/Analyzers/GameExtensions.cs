using Codebreaker.GameAPIs.Analyzers;
using Codebreaker.GameAPIs.Contracts;
using Codebreaker.GameAPIs.Models;

namespace Codebreaker.GameAPIs.Extensions;
public static class GameExtensions
{
    public static void ApplyMove<TField, TResult>(this IGame<TField, TResult> game, IList<TField> guesses, int moveNumber)
        where TResult: struct
    {
        ColorGameMoveAnalyzer GetColorGameMoveAnalyzer()
        {
            if (game is not IGame<ColorField, ColorResult> colorGame)
                throw new ArgumentException("Invalid game type");
            if (guesses is not IList<ColorField> colorGuesses)
                throw new ArgumentException("Invalid guess types");

            return new ColorGameMoveAnalyzer(colorGame, colorGuesses, moveNumber);
        }

        SimpleGameMoveAnalyzer GetSimpleGameMoveAnalyzer()
        {
            if (game is not IGame<ColorField, SimpleColorResult> simpleColorGame)
                throw new ArgumentException("Invalid game type");
            if (guesses is not IList<ColorField> simpleColorGuesses)
                throw new ArgumentException("Invalid guess types");

            return new SimpleGameMoveAnalyzer(simpleColorGame, simpleColorGuesses, moveNumber);
        }

        ShapeGameMoveAnalyzer GetShapeGameMoveAnalyzer()
        {
            if (game is not IGame<ShapeAndColorField, ShapeAndColorResult> shapeGame)
                throw new ArgumentException("Invalid game type");
            if (guesses is not IList<ShapeAndColorField> shapeGuesses)
                throw new ArgumentException("Invalid guess types");

            return new ShapeGameMoveAnalyzer(shapeGame, shapeGuesses, moveNumber);
        }


        IGameMoveAnalyzer analyzer = game.GameType switch
        {
            GameTypes.Game6x4 => GetColorGameMoveAnalyzer(),
            GameTypes.Game8x5 => GetColorGameMoveAnalyzer(),
            GameTypes.Game6x4Mini => GetSimpleGameMoveAnalyzer(),
            GameTypes.Game5x5x4 => GetShapeGameMoveAnalyzer(),
            _ => throw new ArgumentException("Invalid game type")
        };

        analyzer.ApplyMove();
    }
}
