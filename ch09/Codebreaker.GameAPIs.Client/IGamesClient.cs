using Codebreaker.GameAPIs.Client.Models;

namespace Codebreaker.GameAPIs.Client;
public interface IGamesClient
{
    Task<GameInfo?> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<IEnumerable<GameInfo>> GetGamesAsync(GamesQuery query, CancellationToken cancellationToken = default);
    Task<(string[] Results, bool Ended, bool IsVictory)> SetMoveAsync(Guid gameId, string playerName, GameType gameType, int moveNumber, string[] guessPegs, CancellationToken cancellationToken = default);
    Task<(Guid GameId, int NumberCodes, int MaxMoves, IDictionary<string, string[]> FieldValues)> StartGameAsync(GameType gameType, string playerName, CancellationToken cancellationToken = default);
}