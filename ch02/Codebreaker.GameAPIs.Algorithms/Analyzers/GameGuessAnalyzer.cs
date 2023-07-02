using Codebreaker.GameAPIs.Contracts;

namespace Codebreaker.GameAPIs.Analyzers;

public abstract class GameGuessAnalyzer<TField, TResult> : IGameGuessAnalyzer
    where TResult : struct
{
    protected readonly IGame<TField> _game;
    private readonly int _moveNumber;

    protected IList<TField> Guesses { get; private set; }

    protected GameGuessAnalyzer(IGame<TField> game, IList<TField> guesses, int moveNumber)
    {
        _game = game;
        Guesses = guesses;
        _moveNumber = moveNumber;
    }

    /// <summary>
    /// Gets the results using Guesses
    /// </summary>
    /// <returns></returns>
    public abstract TResult GetCoreResult();
    public abstract void ValidateGuessValues();

    /// <summary>
    /// Validates the values of the guess with the current state of the game.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown with an invalid number of guesses (HRESULT=4200), an unexpected move number (4300), or invalid values (44xx)</exception>
    private void ValidateGameStateWithGuess()
    {
        /// The number of holes in the game does not match the number of pegs in the move.
        if (_game.NumberCodes != Guesses.Count)
            throw new ArgumentException($"Invalid guess number {Guesses.Count} for {_game.NumberCodes} holes") { HResult = 4200 };

        ValidateGuessValues();

        _game.LastMoveNumber++;

        if (_game.LastMoveNumber != _moveNumber)
        {
            throw new ArgumentException($"Incorrect move number received {_moveNumber}") { HResult = 4300 };
        }
    }

    /// <summary>
    /// Sets the game end information if the guess is correct, or the maximum number of moves reached.
    /// Implement this method in concrete guess analyzers.
    /// </summary>
    /// <param name="result">The result of the guess analysis</param>
    public abstract void SetGameEndInformation(TResult result);

    public string GetResult()
    {
        ValidateGameStateWithGuess();
        TResult result = GetCoreResult();
        SetGameEndInformation(result);
        return result.ToString() ?? throw new InvalidOperationException();
    }
}
