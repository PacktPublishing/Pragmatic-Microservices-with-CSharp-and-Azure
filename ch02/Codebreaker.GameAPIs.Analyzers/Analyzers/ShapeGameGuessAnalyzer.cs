namespace Codebreaker.GameAPIs.Analyzers;

public class ShapeGameGuessAnalyzer(IGame game, ShapeAndColorField[] guesses, int moveNumber) : GameGuessAnalyzer<ShapeAndColorField, ShapeAndColorResult>(game, guesses, moveNumber)
{
    protected override void ValidateGuessValues()
    {
        // check for valid colors
        if (!Guesses.Select(f => f.Color.ToString())
            .Any(color => _game.FieldValues[FieldCategories.Colors]
            .Contains(color)))
            throw new ArgumentException("The guess contains an invalid color") { HResult = 4402 };

        // check for valid shapes
        if (!Guesses.Select(f => f.Shape.ToString())
            .Any(shape => _game.FieldValues[FieldCategories.Shapes]
            .Contains(shape)))
            throw new ArgumentException("The guess contains an invalid shape") { HResult = 4403 };
    }

    protected override ShapeAndColorResult GetCoreResult()
    {
        // Check black, white and blue keyPegs
        List<ShapeAndColorField> codesToCheck = new(_game.Codes.ToPegs<ShapeAndColorField>());
        List<ShapeAndColorField> guessPegsToCheck = new(Guesses);
        List<ShapeAndColorField> remainingCodesToCheck = [];
        List<ShapeAndColorField> remainingGuessPegsToCheck = [];

        byte black = 0;
        byte white = 0;
        byte blue = 0;

        // check for black (correct color and shape at the correct position)
        // add the remaining codes and guess pegs to the remaining lists to check for white and blue keyPegs
        for (int i = 0; i < guessPegsToCheck.Count; i++)
        {
            if (guessPegsToCheck[i] == codesToCheck[i])
            {
                black++;
            }
            else // the codes and the guess pegs need to be checked again for the blue and white keyPegs
            {
                remainingCodesToCheck.Add(codesToCheck[i]);
                remainingGuessPegsToCheck.Add(guessPegsToCheck[i]);
            }
        }

        codesToCheck = remainingCodesToCheck;
        remainingCodesToCheck = new(codesToCheck);
        guessPegsToCheck = remainingGuessPegsToCheck;
        remainingGuessPegsToCheck = [];

        // check for white (correct pair at a wrong position)
        // and add the remaining codes and guess pegs to the remaining lists to check for blue keyPegs
        for (int i = 0; i < guessPegsToCheck.Count; i++)
        {
            ShapeAndColorField? codeField = codesToCheck.FirstOrDefault(c => c == guessPegsToCheck[i]);
            if (codeField is not null)
            {
                white++;
                codesToCheck.Remove(codeField); // remove for the white check
                remainingCodesToCheck.Remove(codeField); // remove for the blue check
            }
            else
            {
                remainingGuessPegsToCheck.Add(guessPegsToCheck[i]);  // add for the blue check
            }
        }

        codesToCheck = remainingCodesToCheck;
        guessPegsToCheck = remainingGuessPegsToCheck;

        // check blue (either the shape or the color is in the correct position but with a wrong paired element)
        for (int i = 0; i < guessPegsToCheck.Count; i++)
        {
            if (guessPegsToCheck[i].Shape == codesToCheck[i].Shape || 
                guessPegsToCheck[i].Color == codesToCheck[i].Color)
            {
                blue++;
            }
        }

        ShapeAndColorResult resultPegs = new(black, white, blue);

        if ((resultPegs.Correct + resultPegs.WrongPosition + resultPegs.ColorOrShape) > _game.NumberCodes)
            throw new InvalidOperationException("There are more keyPegs than codes"); // Should not be the case

        return resultPegs;
    }

    protected override void SetGameEndInformation(ShapeAndColorResult result)
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
