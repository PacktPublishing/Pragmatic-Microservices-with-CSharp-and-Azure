namespace Codebreaker.GameAPIs.Analyzers;

public class SimpleGameGuessAnalyzer(IGame game, ColorField[] guesses, int moveNumber) : GameGuessAnalyzer<ColorField, SimpleColorResult>(game, guesses, moveNumber)
{
    protected override void ValidateGuessValues()
    {
        if (Guesses.Any(guessPeg => !_game.FieldValues[FieldCategories.Colors].Contains(guessPeg.ToString())))
            throw new ArgumentException("The guess contains an invalid value") { HResult = 4400 };
    }

    protected override SimpleColorResult GetCoreResult()
    {
        // Check black and white keyPegs
        List<ColorField> codesToCheck = new(_game.Codes.ToPegs<ColorField>());
        List<ColorField> guessPegsToCheck = new(Guesses);

        var results = Enumerable.Repeat(ResultValue.Incorrect, 4).ToArray();

        for (int i = 0; i < results.Length; i++)
        {
            results[i] = ResultValue.Incorrect;
        }

        // check black
        for (int i = 0; i < _game.Codes.Length; i++)
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

    protected override void SetGameEndInformation(SimpleColorResult result)
    {
        bool allCorrect = result.Results.Any(r => r == ResultValue.CorrectColor);

        if (allCorrect || _game.LastMoveNumber >= _game.MaxMoves)
        {
            _game.EndTime = DateTime.UtcNow;
            _game.Duration = _game.EndTime - _game.StartTime;
        }
        if (allCorrect)
        {
            _game.IsVictory = true;
        }
    }
}
