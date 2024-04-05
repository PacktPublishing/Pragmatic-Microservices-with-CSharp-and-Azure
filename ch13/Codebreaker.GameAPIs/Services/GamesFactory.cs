using Codebreaker.GameAPIs.Analyzers;

namespace Codebreaker.GameAPIs.Services;

/// <summary>
/// A factory class for creating instances of the <see cref="Game"/> class.
/// </summary>
public static class GamesFactory
{
    private static readonly string[] s_colors6 = [Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple, Colors.Orange];
    private static readonly string[] s_colors8 = [Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple, Colors.Orange, Colors.Pink, Colors.Brown];
    private static readonly string[] s_colors5 = [Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple];
    private static readonly string[] s_shapes5 = [Shapes.Circle, Shapes.Square, Shapes.Triangle, Shapes.Star, Shapes.Rectangle];

    /// <summary>
    /// Creates a game object with specified gameType and playerName.
    /// </summary>
    /// <param name="gameType">The type of game to be created.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <returns>The created game object.</returns>
    public static Game CreateGame(string gameType, string playerName)
    {
        Game Create6x4SimpleGame() =>
            new(Guid.NewGuid(), gameType, playerName,  DateTime.UtcNow, 4, 12)
            {
                FieldValues = new Dictionary<string, IEnumerable<string>>()
                {
                    { FieldCategories.Colors, s_colors6 }
                },
                Codes = Random.Shared.GetItems(s_colors6, 4)
            };

        Game Create6x4Game() =>
            new(Guid.NewGuid(), gameType, playerName, DateTime.UtcNow, 4, 12)
            {
                FieldValues = new Dictionary<string, IEnumerable<string>>()
                {
                    { FieldCategories.Colors, s_colors6 }
                },
                Codes = Random.Shared.GetItems(s_colors6, 4)
            };

        Game Create8x5Game() =>
            new(Guid.NewGuid(), gameType, playerName, DateTime.UtcNow, 5, 12)
            {
                FieldValues = new Dictionary<string, IEnumerable<string>>()
                {
                    { FieldCategories.Colors, s_colors8 }
                },
                Codes = Random.Shared.GetItems(s_colors8, 5)
            };

        Game Create5x5x4Game() =>
            new(Guid.NewGuid(), gameType, playerName, DateTime.UtcNow, 4, 14)
            {
                FieldValues = new Dictionary<string, IEnumerable<string>>()
                {
                    { FieldCategories.Colors, s_colors5 },
                    { FieldCategories.Shapes, s_shapes5 }
                },
                Codes = Random.Shared.GetItems(s_shapes5, 4)
                    .Zip(Random.Shared.GetItems(s_colors5, 4), (shape, color) => (Shape: shape, Color: color))
                    .Select(item => string.Join(';', item.Shape, item.Color))
                    .ToArray()
            };
        
        return gameType switch
        {
            GameTypes.Game6x4Mini => Create6x4SimpleGame(),
            GameTypes.Game6x4 => Create6x4Game(),
            GameTypes.Game8x5 => Create8x5Game(),
            GameTypes.Game5x5x4 => Create5x5x4Game(),
            _ => throw new CodebreakerException("Invalid game type") { Code = CodebreakerExceptionCodes.InvalidGameType }
        };
    }

    /// <summary>
    /// Applies a player's move to a game and returns a <see cref="Move"/> object that encapsulates the player's guess and the result of the guess.
    /// Returns the Move and updates the Game
    /// </summary>
    /// <param name="game">The game to apply the move to.</param>
    /// <param name="guesses">The player's guesses.</param>
    /// <param name="moveNumber">The move number.</param>
    /// <returns>A <see cref="Move"/> object that encapsulates the player's guess and the result of the guess.</returns>
    public static Move ApplyMove(this Game game, string[] guesses, int moveNumber)
    {
        static TField[] GetGuesses<TField>(IEnumerable<string> guesses)
            where TField : IParsable<TField> => guesses
                .Select(g => TField.Parse(g, default))
                .ToArray();

        string[] GetColorGameGuessAnalyzerResult()
        {
            ColorGameGuessAnalyzer analyzer = new(game, GetGuesses<ColorField>(guesses), moveNumber);
            return analyzer.GetResult().ToStringResults();
        }

        string[] GetSimpleGameGuessAnalyzerResult()
        {
            SimpleGameGuessAnalyzer analyzer = new(game, GetGuesses<ColorField>(guesses), moveNumber);
            return analyzer.GetResult().ToStringResults();
        }

        string[] GetShapeGameGuessAnalyzerResult()
        {
            ShapeGameGuessAnalyzer analyzer = new(game, GetGuesses<ShapeAndColorField>(guesses), moveNumber);
            return analyzer.GetResult().ToStringResults();
        }

        string[] results = game.GameType switch
        {
            GameTypes.Game6x4 => GetColorGameGuessAnalyzerResult(),
            GameTypes.Game8x5 => GetColorGameGuessAnalyzerResult(),
            GameTypes.Game6x4Mini => GetSimpleGameGuessAnalyzerResult(),
            GameTypes.Game5x5x4 => GetShapeGameGuessAnalyzerResult(),
            _ => throw new CodebreakerException("Invalid game type") { Code = CodebreakerExceptionCodes.InvalidGameType }
        };

        Move move = new(Guid.NewGuid(), moveNumber)
        {
            GuessPegs = guesses,
            KeyPegs = results
        };

        game.Moves.Add(move);
        return move;
    }
}
