namespace Codebreaker.GameAPIs.Client;

public interface IGamesClient
{
    Task<GameInfo?> GetGameAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GameInfo>> GetGamesAsync(GamesQuery query, CancellationToken cancellationToken = default);
    Task<(string[] Results, bool Ended, bool IsVictory)> SetMoveAsync(Guid id, string playerName, GameType gameType, int moveNumber, string[] guessPegs, CancellationToken cancellationToken = default);
    Task<(Guid Id, int NumberCodes, int MaxMoves, IDictionary<string, string[]> FieldValues)> StartGameAsync(GameType gameType, string playerName, CancellationToken cancellationToken = default);
}