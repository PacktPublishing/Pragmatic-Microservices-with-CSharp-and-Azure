namespace Codebreaker.GameAPIs.Analyzers;

public class ColorGameGuessAnalyzer(IGame game, ColorField[] guesses, int moveNumber) : GameGuessAnalyzer<ColorField, ColorResult>(game, guesses, moveNumber)
{
    protected override void ValidateGuessValues()
    {
        if (Guesses.Any(guessPeg => !_game.FieldValues[FieldCategories.Colors].Contains(guessPeg.ToString())))
        {
            string guesses = string.Join(", ", Guesses.Select(g => g.ToString()));
            string fields = string.Join(", ", _game.FieldValues[FieldCategories.Colors]);
            throw new ArgumentException($"The guess contains an invalid value. Guesses: {guesses}, fields: {fields}") { HResult = 4400 };
        }
    }

    protected override ColorResult GetCoreResult()
    {
        // Check black and white keyPegs
        List<ColorField> codesToCheck = new(_game.Codes.ToPegs<ColorField>());
        List<ColorField> guessPegsToCheck = new(Guesses);
        int black = 0;
        List<string> whitePegs = [];

        // check black
        for (int i = 0; i < guessPegsToCheck.Count; i++)
        {
            if (guessPegsToCheck[i] == codesToCheck[i])
            {
                black++;
                codesToCheck.RemoveAt(i);
                guessPegsToCheck.RemoveAt(i);
                i--;
            }
        }

        // check white
        foreach (ColorField value in guessPegsToCheck)
        {
            // value not in code
            if (!codesToCheck.Contains(value))
                continue;

            // value peg was already added to the white pegs often enough
            // (max. the number in the codeToCheck)
            if (whitePegs.Count(x => x == value.Color) == codesToCheck.Count(x => x == value))
                continue;

            whitePegs.Add(value.Color);
        }

        ColorResult resultPegs = new(black, whitePegs.Count);
        if (resultPegs.Correct + resultPegs.WrongPosition > _game.NumberCodes)
        {
            throw new InvalidOperationException("More key pegs than codes");
        }

        return resultPegs;
    }

    protected override void SetGameEndInformation(ColorResult result)
    {
        bool allCorrect = result.Correct == _game.NumberCodes;
        if (allCorrect || _game.LastMoveNumber >= _game.MaxMoves)
        {
            _game.EndTime = DateTime.UtcNow;
            _game.Duration = _game.EndTime - _game.StartTime;
        }
        _game.IsVictory = allCorrect;
    }
}
