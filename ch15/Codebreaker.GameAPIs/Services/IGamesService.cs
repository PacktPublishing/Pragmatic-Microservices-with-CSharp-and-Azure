namespace Codebreaker.GameAPIs.Services;

/// <summary>
/// Interface for Game Service
/// </summary>
public interface IGamesService
{
    /// <summary>
    /// Start a new game
    /// </summary>
    /// <param name="gameType">type of the game</param>
    /// <param name="playerName">name of the player</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>new game</returns>
    Task<Game> StartGameAsync(string gameType, string playerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// set new moves
    /// </summary>
    /// <param name="id">the id of the game</param>
    /// <param name="gameType">the type of the game</param>
    /// <param name="guesses">an enumerable guesses of strings</param>
    /// <param name="moveNumber">the number of the move</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>tuple consisting of the updated game and its result</returns>
    Task<(Game Game, Move Move)> SetMoveAsync(Guid id, string gameType, string[] guesses, int moveNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the Game by id
    /// </summary>
    /// <param name="id">the id of the game</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>the game with the given id or null if the game was not found</returns>
    ValueTask<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a game with the given id
    /// </summary>
    /// <param name="id">the id of the game</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns></returns>
    Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ends and returns the game with the result
    /// </summary>
    /// <param name="id">the game id to end</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>ended game</returns>
    Task<Game> EndGameAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a list of games as an IEnumerable of Game
    /// </summary>
    /// <param name="gamesQuery">optional games query</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>IEnumerable of Game</returns>
    Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default);
}
