namespace Codebreaker.GameAPIs.Data;

/// <summary>
/// Acts as a proxy for database operations related to games, implementing the IGamesRepository interface.
/// </summary>
/// <typeparam name="TContext">Represents a specific type of database context that supports game-related operations.</typeparam>
/// <param name="context">Provides the instance of the database context used to perform operations on game data.</param>
internal class DataContextProxy<TContext>(TContext context) : IGamesRepository
    where TContext : DbContext, IGamesRepository
{
    public Task AddGameAsync(Game game, CancellationToken cancellationToken = default) => context.AddGameAsync(game, cancellationToken);
    public Task AddMoveAsync(Game game, Move move, CancellationToken cancellationToken = default) => context.AddMoveAsync(game, move, cancellationToken);
    public Task<bool> DeleteGameAsync(Guid id, CancellationToken cancellationToken = default) => context.DeleteGameAsync(id, cancellationToken);
    public Task<Game?> GetGameAsync(Guid id, CancellationToken cancellationToken = default) => context.GetGameAsync(id, cancellationToken);
    public Task<IEnumerable<Game>> GetGamesAsync(GamesQuery gamesQuery, CancellationToken cancellationToken = default) => context.GetGamesAsync(gamesQuery, cancellationToken);
    public Task<Game> UpdateGameAsync(Game game, CancellationToken cancellationToken = default) => context.UpdateGameAsync(game, cancellationToken);

    /// <summary>
    /// Updates the database asynchronously by applying migrations if the context is a GamesSqlServerContext.
    /// </summary>
    /// <param name="logger">Used to log information about the database migration process.</param>
    /// <returns>This method returns a Task representing the asynchronous operation.</returns>
    public async Task UpdateDatabaseAsync(ILogger logger)
    {
        if (context is GamesSqlServerContext gamesSqlContext)
        {
            await gamesSqlContext.Database.MigrateAsync();
            logger.LogInformation("Sql Server database migration applied");
        }
    }
}
