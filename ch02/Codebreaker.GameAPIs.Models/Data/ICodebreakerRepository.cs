namespace Codebreaker.GameAPIs.Data;

public interface ICodebreakerRepository
{
    Task AddGameAsync(Game game, CancellationToken cancellationToken = default);
    Task UpdateGameAsync(Game game, CancellationToken cancellationToken = default);
    Task DeleteGameAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<Game?> GetGameAsync(Guid gameId, bool withTracking = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetGamesByDateAsync(GameType gameType, DateOnly date, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetMyGamesAsync(string playerName, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetMyRunningGamesAsync(string playerName, CancellationToken cancellationToken = default);
}
