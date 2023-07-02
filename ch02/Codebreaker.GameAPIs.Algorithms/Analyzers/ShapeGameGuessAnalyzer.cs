using Codebreaker.GameAPIs.Algorithms.Fields;
using Codebreaker.GameAPIs.Contracts;
using Codebreaker.GameAPIs.Models;

namespace Codebreaker.GameAPIs.Analyzers;

public class ShapeGameGuessAnalyzer : GameGuessAnalyzer<ShapeAndColorField, ShapeAndColorResult>
{
    public ShapeGameGuessAnalyzer(IGame<ShapeAndColorField> game, IList<ShapeAndColorField> guesses, int moveNumber)
        : base(game, guesses, moveNumber)
    {
    }

    public override void ValidateGuessValues()
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

    public override ShapeAndColorResult GetCoreResult()
    {
        // Check black and white keyPegs
        List<ShapeAndColorField> codesToCheck = new(_game.Codes);
        List<ShapeAndColorField> guessPegsToCheck = new(Guesses);
        List<ShapeAndColorField> remainingCodesToCheck = new();
        List<ShapeAndColorField> remainingGuessPegsToCheck = new();

        byte black = 0;
        byte blue = 0;
        byte white = 0;

        // check black (correct color and shape)
        for (int i = 0; i < guessPegsToCheck.Count; i++)
        {
            if (guessPegsToCheck[i] == codesToCheck[i])
            {
                black++;
            }
            else
            {
                remainingCodesToCheck.Add(codesToCheck[i]);
                remainingGuessPegsToCheck.Add(guessPegsToCheck[i]);
            }
        }

        codesToCheck = remainingCodesToCheck;
        remainingCodesToCheck = new(codesToCheck);
        guessPegsToCheck = remainingGuessPegsToCheck;
        remainingGuessPegsToCheck = new();

        // check blue (correct shape and color on a wrong position)
        for (int i = 0; i < guessPegsToCheck.Count; i++)
        {
            ShapeAndColorField? codeField = codesToCheck.FirstOrDefault(c => c == guessPegsToCheck[i]);
            if (codeField is not null)
            {
                blue++;
                remainingCodesToCheck.Remove(codeField); // remove for the white check
            }
            else
            {
                remainingGuessPegsToCheck.Add(guessPegsToCheck[i]);  // add for the white check
            }
        }

        codesToCheck = remainingCodesToCheck;
        guessPegsToCheck = remainingGuessPegsToCheck;

        // check white (either the shape or the color is correct on a wrong position)
        for (int i = 0; i < guessPegsToCheck.Count; i++)
        {
            var colorCodes = codesToCheck.Select(c => c.Color).ToArray();
            var shapeCodes = codesToCheck.Select(c => c.Shape).ToArray();

            if (colorCodes.Contains(guessPegsToCheck[i].Color) || shapeCodes.Contains(guessPegsToCheck[i].Shape))
            {
                white++;
            }
        }

        ShapeAndColorResult resultPegs = new(black, blue, white);

        if ((resultPegs.Correct + resultPegs.WrongPosition + resultPegs.ColorOrShape) > _game.NumberCodes)
            throw new InvalidOperationException("There are more keyPegs than holes"); // Should not be the case

        return resultPegs;
    }

    public override void SetGameEndInformation(ShapeAndColorResult result)
    {
        bool allCorrect = result.Correct == _game.NumberCodes;
        if (allCorrect || _game.LastMoveNumber >= _game.MaxMoves)
        {
            _game.EndTime = DateTime.UtcNow;
            _game.Duration = _game.EndTime - _game.StartTime;
        }
        _game.Won = allCorrect;
    }
}
