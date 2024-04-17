namespace Codebreaker.GameAPIs.Client.Models;

public record class CreateGameResponse(
    Guid Id,
    GameType GameType,
    string PlayerName,
    int NumberCodes,
    int MaxMoves)
{
    public required IDictionary<string, string[]> FieldValues { get; init; }
}
