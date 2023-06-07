using Codebreaker.GameAPIs.Contracts;
using Codebreaker.GameAPIs.Models;

namespace Codebreaker.GameAPIs.Analyzers;

public class ShapeGameMoveAnalyzer : GameMoveAnalyzer<ShapeAndColorField, ShapeAndColorResult>
{
    public ShapeGameMoveAnalyzer(IGame<ShapeAndColorField, ShapeAndColorResult> game, IList<ShapeAndColorField> guesses, int moveNumber)
        : base(game, guesses, moveNumber)
    {
    }

    public override void ValidateGuessPegs()
    {
        // check for valid colors
        if (Guesses.Select(f => f.Color).Any(color => !_game.Fields.Select(f => f.Color).Contains(color)))
            throw new ArgumentException("The guess contains an invalid color") { HResult = 4402 };

        // check for valid shapes
        if (Guesses.Select(f => f.Shape).Any(shape => !_game.Fields.Select(c => c.Shape).Contains(shape)))
            throw new ArgumentException("The guess contains an invalid shape") { HResult = 4403 };
    }

    public override ShapeAndColorResult GetResult()
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

        if ((resultPegs.Correct + resultPegs.WrongPosition + resultPegs.ColorOrShape) > _game.Holes)
            throw new InvalidOperationException("There are more keyPegs than holes"); // Should not be the case

        return resultPegs;
    }

    public override void SetEndInformation()
    {
        bool allCorrect = _game.Moves.Last().KeyPegs?.Correct == _game.Holes;
        if (allCorrect || _game.Moves.Count >= _game.MaxMoves)
        {
            _game.EndTime = DateTime.UtcNow;
            _game.Duration = _game.EndTime - _game.StartTime;
        }
        _game.Won = allCorrect;
    }
}
