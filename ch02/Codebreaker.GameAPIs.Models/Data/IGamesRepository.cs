namespace Codebreaker.GameAPIs.Data;

public interface IGamesRepository
{
    Task AddGameAsync(Game game, CancellationToken cancellationToken = default);
    Task AddMoveAsync(Game game, Move move, CancellationToken cancellationToken = default);
    Task<bool> DeleteGameAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<Game?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default);
    Task<Game> UpdateGameAsync(Game game, CancellationToken cancellationToken = default);
}
