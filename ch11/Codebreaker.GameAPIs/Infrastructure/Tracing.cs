using System.Diagnostics;

namespace Codebreaker.GameAPIs.Infrastructure;

internal static class Tracing
{
    internal const string ActivitySourceName = "Codebreaker.GameAPIs";
    internal const string ActivityVersion = "1.0.0";
    internal static ActivitySource _activitySource = new(ActivitySourceName, ActivityVersion);

    public static Activity? StartGame(Guid id, string gameType)
    {
        Activity? activity = _activitySource.StartActivity(nameof(StartGame), ActivityKind.Server); 
        activity?.AddBaggage("id", id.ToString());
        activity?.AddTag("gameType", gameType);
        return activity;
    }

    public static Activity? SetMove(Guid id, string gameType)
    {
        Activity? activity = _activitySource.StartActivity(nameof(SetMove), ActivityKind.Server); ;
        activity?.AddBaggage("id", id.ToString());
        activity?.AddTag("gameType", gameType);
        return activity;
    }
}
