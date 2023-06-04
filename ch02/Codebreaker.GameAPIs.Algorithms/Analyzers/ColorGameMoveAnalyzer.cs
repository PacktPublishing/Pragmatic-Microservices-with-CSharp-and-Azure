using Codebreaker.GameAPIs.Contracts;
using Codebreaker.GameAPIs.Models;

namespace Codebreaker.GameAPIs.Analyzers;

public class ColorGameMoveAnalyzer : GameMoveAnalyzer<ColorField, ColorResult>
{
    public ColorGameMoveAnalyzer(IGame<ColorField, ColorResult> game, IList<ColorField> guesses, int moveNumber)
        : base(game, guesses, moveNumber)
    {
    }

    public override void ValidateGuessPegs()
    {
        if (Guesses.Any(guessPeg => !_game.Fields.Contains(guessPeg)))
            throw new ArgumentException("The guess contains an invalid value");
    }

    public override ColorResult GetResult()
    {
        // Check black and white keyPegs
        List<ColorField> codesToCheck = new(_game.Codes);
        List<ColorField> guessPegsToCheck = new(Guesses);
        int black = 0;
        List<string> whitePegs = new();

        // check black
        for (int i = 0; i < guessPegsToCheck.Count; i++)
            if (guessPegsToCheck[i] == codesToCheck[i])
            {
                black++;
                codesToCheck.RemoveAt(i);
                guessPegsToCheck.RemoveAt(i);
                i--;
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
        if (resultPegs.Correct + resultPegs.WrongPosition > _game.Holes)
        {
            throw new InvalidOperationException("More key pegs than holes");
        }

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
