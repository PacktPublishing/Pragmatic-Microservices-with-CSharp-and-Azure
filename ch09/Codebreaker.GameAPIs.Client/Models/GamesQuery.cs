namespace Codebreaker.GameAPIs.Client.Models;

/// <summary>
/// Filter based on game type, player name, date, and ended
/// </summary>
/// <param name="GameType">The game type with one of the <see cref="GameType"/>enum values</param>
/// <param name="PlayerName">The name of the player</param>
/// <param name="Date">The start time of the game</param>
/// <param name="Ended">Only ended or running games</param>
public record class GamesQuery(
    GameType? GameType = default, 
    string? PlayerName = default, 
    DateOnly? Date = default, 
    bool? Ended = false)
{
    public string AsUrlQuery()
    {
        var queryString = "?";

        // Add condition for gameType
        if (GameType != null)
        {
            queryString += $"gameType={GameType}&";
        }

        // Add condition for playerName
        if (PlayerName != null)
        {
            queryString += $"playerName={Uri.EscapeDataString(PlayerName)}&";
        }

        // Add condition for date
        if (Date != null)
        {
            string dateString = Date.Value.ToString("yyyy-MM-dd");
            queryString += $"date={dateString}&";
        }

        // Add condition for ended
        if (Ended != null)
        {
            queryString += $"ended={Ended}&";
        }

        // Remove the last character if it is an ampersand character
        queryString = queryString.TrimEnd('&');

        return queryString;
    }
}
