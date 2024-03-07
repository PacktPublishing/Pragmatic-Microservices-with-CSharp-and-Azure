using System.Globalization;
using System.Text;

namespace Codebreaker.GameAPIs.Data;

public record GamesQuery(
    string? GameType = default, 
    string? PlayerName = default, 
    DateOnly? Date = default,
    bool Ended = true,
    bool RunningOnly = false)
{
    public override string ToString()
    {
        StringBuilder sb = new();
        if (GameType != default)
        {
            sb.Append($"GameType:{GameType},");
        }
        if (PlayerName != default)
        {
            sb.Append($"PlayerName:{PlayerName},");
        }
        if (Date.HasValue)
        {
            sb.Append($"Date:{Date.Value.ToString("d", CultureInfo.InvariantCulture)},");
        }
        sb.Append($"Ended:{Ended},");
        sb.Append($"RunningOnly:{RunningOnly}");
        return sb.ToString().TrimEnd(',', ' ');
    }
}
