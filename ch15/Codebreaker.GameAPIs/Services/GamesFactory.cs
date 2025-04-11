using Codebreaker.GameAPIs.Analyzers;

namespace Codebreaker.GameAPIs.Services;

/// <summary>
/// Creates a new game based on the specified type and player name, returning a configured game instance.
/// Throws an exception for invalid game types.
/// </summary>
public static class GamesFactory
{
    private static readonly string[] s_colors6 = [Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple, Colors.Orange];
    private static readonly string[] s_colors8 = [Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple, Colors.Orange, Colors.Pink, Colors.Brown];
    private static readonly string[] s_colors5 = [Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple];
    private static readonly string[] s_shapes5 = [Shapes.Circle, Shapes.Square, Shapes.Triangle, Shapes.Star, Shapes.Rectangle];


    /// <summary>
    /// Creates a new game based on the specified type and player name.
    /// </summary>
    /// <param name="gameType">Specifies the type of game to be created, determining its rules and structure.</param>
    /// <param name="playerName">Indicates the name of the player participating in the game.</param>
    /// <returns>Returns a new game instance configured according to the specified type.</returns>
    /// <exception cref="CodebreakerException">Thrown when an invalid game type is provided.</exception>
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
                Codes = [.. Random.Shared.GetItems(s_shapes5, 4)
                    .Zip(Random.Shared.GetItems(s_colors5, 4), (shape, color) => (Shape: shape, Color: color))
                    .Select(item => string.Join(';', item.Shape, item.Color))]
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
    /// Applies a move to the game based on the provided guesses and move number, returning the created move object.
    /// </summary>
    /// <param name="game">Represents the current game instance to which the move is being applied.</param>
    /// <param name="guesses">Contains the player's guesses that will be evaluated against the game's rules.</param>
    /// <param name="moveNumber">Indicates the sequence number of the move being applied in the game.</param>
    /// <returns>Returns the newly created move object that includes the guesses and the results of the analysis.</returns>
    /// <exception cref="CodebreakerException">Thrown when the game type is invalid and does not match any predefined types.</exception>
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
