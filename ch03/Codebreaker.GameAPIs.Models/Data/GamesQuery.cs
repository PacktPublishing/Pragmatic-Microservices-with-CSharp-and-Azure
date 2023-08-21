namespace Codebreaker.GameAPIs.Data;

public record GamesQuery(
    string? GameType = default, 
    string? PlayerName = default, 
    DateOnly? Date = default,
    bool Ended = true,
    bool RunningOnly = false);
