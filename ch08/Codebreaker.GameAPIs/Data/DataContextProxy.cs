using Microsoft.EntityFrameworkCore;

namespace Codebreaker.GameAPIs.Data;

public class DataContextProxy<TContext>(TContext context) : IGamesRepository
    where TContext: DbContext, IGamesRepository
{
    public Task AddGameAsync(Game game, CancellationToken cancellationToken = default) => context.AddGameAsync(game, cancellationToken);
    public Task AddMoveAsync(Game game, Move move, CancellationToken cancellationToken = default) => context.AddMoveAsync(game, move, cancellationToken);
    public Task<bool> DeleteGameAsync(Guid id, CancellationToken cancellationToken = default) => context.DeleteGameAsync(id, cancellationToken);
    public Task<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default) => context.GetGameAsync(id, cancellationToken);
    public Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default) => context.GetGamesAsync(gamesQuery, cancellationToken);
    public Task<Game> UpdateGameAsync(Game game, CancellationToken cancellationToken = default) => context.UpdateGameAsync(game, cancellationToken);
}
