using Codebreaker.GameAPIs.Contracts;
using Codebreaker.GameAPIs.Models;

namespace Codebreaker.GameAPIs.Analyzers;

public class SimpleGameMoveAnalyzer : GameMoveAnalyzer<ColorField, SimpleColorResult>
{
    public SimpleGameMoveAnalyzer(IGame<ColorField, SimpleColorResult> game, IList<ColorField> guesses, int moveNumber)
        : base(game, guesses, moveNumber)
    {
    }

    public override void ValidateGuessPegs()
    {
        if (Guesses.Any(guessPeg => _game.Fields.Contains(guessPeg)))
            throw new ArgumentException("The guess contains an invalid value");
    }

    public override SimpleColorResult GetResult()
    {
        // Check black and white keyPegs
        List<ColorField> codesToCheck = new(_game.Codes);
        List<ColorField> guessPegsToCheck = new(Guesses);

        var results = Enumerable.Repeat(ResultValue.Incorrect, 4).ToArray();

        for (int i = 0; i < results.Length; i++)
        {
            results[i] = ResultValue.Incorrect;
        }

        // check black
        for (int i = 0; i < _game.Codes.Count; i++)
        {
            // check black
            if (guessPegsToCheck[i] == codesToCheck[i])
            {
                results[i] = ResultValue.CorrectPositionAndColor;
            }
            else // check white
            {
                if (codesToCheck.Contains(codesToCheck[i]) && results[i] == ResultValue.Incorrect)
                {
                    results[i] = ResultValue.CorrectColor;
                }
            }
        }

        return new SimpleColorResult(results);
    }

    public override void SetEndInformation()
    {
        SimpleColorResult result = _game.Moves.Last().KeyPegs ?? throw new InvalidOperationException();
        bool allCorrect = result.Results.Any(r => r == ResultValue.CorrectColor);

        if (allCorrect || _game.Moves.Count >= _game.MaxMoves)
        {
            _game.EndTime = DateTime.UtcNow;
            _game.Duration = _game.EndTime - _game.StartTime;
        }
        if (allCorrect)
        {
            _game.Won = true;
        }
    }
}
