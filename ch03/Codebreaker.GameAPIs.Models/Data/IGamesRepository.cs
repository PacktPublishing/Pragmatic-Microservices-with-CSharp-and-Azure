namespace Codebreaker.GameAPIs.Data;

public interface IGamesRepository
{
    Task AddGameAsync(Game game, CancellationToken cancellationToken = default);
    Task UpdateGameAsync(Game game, CancellationToken cancellationToken = default);
    Task DeleteGameAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<Game?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetGamesByDateAsync(string gameType, DateOnly date, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetMyGamesAsync(string playerName, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetMyRunningGamesAsync(string playerName, CancellationToken cancellationToken = default);
}
