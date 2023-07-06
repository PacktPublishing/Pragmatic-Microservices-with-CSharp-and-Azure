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
    /// <param name="gameId">the id of the game</param>
    /// <param name="guesses">an enumerable guesses of strings</param>
    /// <param name="moveNumber">the number of the move</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>tuple consisting of the updated game and its result</returns>
    Task<(Game Game, Move Move)> SetMoveAsync(Guid gameId, string[] guesses, int moveNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the Game by id
    /// </summary>
    /// <param name="id">the id of the game</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>the game with the given id</returns>
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
    /// <param name="gameId">the game id to end</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>ended game</returns>
    Task<Game> EndGameAsync(Guid gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get ranked games by date
    /// </summary>
    /// <param name="gameType">type of the game</param>
    /// <param name="date">date of the game</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>list of ranked games</returns>
    Task<IEnumerable<Game>> GetGamesRankByDateAsync(GameType gameType, DateOnly date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get last games which did not end of the player
    /// </summary>
    /// <param name="playerName">name of the player</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>list of last running games by playerName</returns>
    Task<IEnumerable<Game>> GetRunningGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all the completed games by the player
    /// </summary>
    /// <param name="playerName">name of the player</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>list of all games played by playerName</returns>
    Task<IEnumerable<Game>> GetCompletedGamesByPlayerAsync(string playerName, CancellationToken cancellationToken = default);
}
