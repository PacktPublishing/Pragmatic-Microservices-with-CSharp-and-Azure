using Codebreaker.GameAPIs.Contracts;

namespace Codebreaker.GameAPIs.Analyzers;

public abstract class GameMoveAnalyzer<TField, TResult> : IGameMoveAnalyzer
    where TResult : struct
{
    protected readonly IGame<TField, TResult> _game;
    private readonly IList<TField> _guesses;
    private readonly int _moveNumber;

    protected IList<TField> Guesses => _guesses;

    protected GameMoveAnalyzer(IGame<TField, TResult> game, IList<TField> guesses, int moveNumber)
    {
        _game = game;
        _guesses = guesses;
        _moveNumber = moveNumber;
    }

    public abstract TResult GetResult();
    public abstract void ValidateGuessPegs();

    private void ValidateMove()
    {
        /// The number of holes in the game does not match the number of pegs in the move.
        if (_game.Holes != _guesses.Count)
            throw new ArgumentException($"Invalid guess number {_guesses.Count} for {_game.Holes} holes");

        ValidateGuessPegs();

        _game.LastMoveNumber++;

        if (_game.LastMoveNumber != _moveNumber)
            throw new ArgumentException($"Incorrect move number received {_moveNumber}");
    }

    public abstract void SetEndInformation();

    public void ApplyMove()
    {
        ValidateMove();
        var result = GetResult();
        var move = _game.CreateMove(_guesses, result, _moveNumber);
        _game.Moves.Add(move);
        SetEndInformation();
    }
}
