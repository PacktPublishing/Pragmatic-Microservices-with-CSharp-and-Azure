namespace Codebreaker.GameAPIs.Client.Models;

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
