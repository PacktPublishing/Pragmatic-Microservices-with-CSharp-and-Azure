namespace Codebreaker.GameAPIs.Contracts;

public interface IGameGuessAnalyzer<TResult>
{
    /// <summary>
    /// Returns the result of the guesses using the current state of the game.
    /// </summary>
    /// <returns>The result of the move that is parsable</returns>
    /// <exception cref="ArgumentException" HResult="4200">Thrown when the number of guesses does not match the number of codes in the game</exception>
    /// <exception cref="ArgumentException" HRESULT="4300">Thrown when the move number is not the next move number in the game</exception>
    /// <exception cref="ArgumentException" HRESULT="4400..4404">Thrown when the guess contains an invalid value</exception></exception>
    TResult GetResult();
}
