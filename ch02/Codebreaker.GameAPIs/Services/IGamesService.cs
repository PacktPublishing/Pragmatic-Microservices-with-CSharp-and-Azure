namespace Codebreaker.GameAPIs.Services;

public interface IGamesService
{
    Task<Game> StartGameAsync(string gameType, string playerName, CancellationToken cancellationToken = default);

    Task<Game> SetMoveAsync(Guid gameId, IEnumerable<string> guesses, int moveNumber, CancellationToken cancellationToken = default);

    ValueTask<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default);

    Task DeleteGameAsync(Guid id, CancellationToken cancellationToken = default);

    // user gives up - end and return the game with the result
    Task<Game> EndGameAsync(Guid gameId);

    Task<IEnumerable<Game>> GetGamesRankByDateAsync(GameType gameType, DateOnly date, CancellationToken cancellationToken = default);

    // get the games from the last 24 hours which are not finished
    Task<IEnumerable<Game>> GetMyLastRunningGamesAsync(string playerName, CancellationToken cancellationToken = default);

    Task<IEnumerable<Game>> GetAllMyGamesAsync(string playerName, CancellationToken cancellationToken = default);
}
