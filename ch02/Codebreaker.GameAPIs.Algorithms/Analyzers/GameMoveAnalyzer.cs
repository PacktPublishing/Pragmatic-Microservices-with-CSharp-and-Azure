using Codebreaker.GameAPIs.Contracts;

namespace Codebreaker.GameAPIs.Analyzers;

public abstract class GameMoveAnalyzer<TField, TResult> : IGameMoveAnalyzer
    where TResult : struct
{
    protected readonly IGame<TField, TResult> _game;
    private readonly int _moveNumber;

    protected IList<TField> Guesses { get; private set; }

    protected GameMoveAnalyzer(IGame<TField, TResult> game, IList<TField> guesses, int moveNumber)
    {
        _game = game;
        Guesses = guesses;
        _moveNumber = moveNumber;
    }

    public abstract TResult GetResult();
    public abstract void ValidateGuessPegs();

    private void ValidateMove()
    {
        /// The number of holes in the game does not match the number of pegs in the move.
        if (_game.Holes != Guesses.Count)
            throw new ArgumentException($"Invalid guess number {Guesses.Count} for {_game.Holes} holes");

        ValidateGuessPegs();

        _game.LastMoveNumber++;

        if (_game.LastMoveNumber != _moveNumber)
            throw new ArgumentException($"Incorrect move number received {_moveNumber}");
    }

    public abstract void SetEndInformation();

    public string ApplyMove()
    {
        ValidateMove();
        TResult result = GetResult();
        var move = _game.CreateMove(Guesses, result, _moveNumber);
        _game.Moves.Add(move);
        SetEndInformation();
        return result.ToString() ?? string.Empty;
    }
}
