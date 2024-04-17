namespace Codebreaker.GameAPIs.Client;

internal static class ActivityExtensions
{
    public static void ErrorEvent(this Activity? activity, string message)
    {
        activity?.AddEvent(new ActivityEvent("Error", tags: new ActivityTagsCollection()
            {
                new KeyValuePair<string, object?>("otel.status_Code", "Error"),
                new KeyValuePair<string, object?>("otel.status_description", message)
            }));
    }

    public static void GameCreatedEvent(this Activity? activity, string gameId, string gameType)
    {
        activity?.SetBaggage("gameId", gameId);
        activity?.AddEvent(
            new ActivityEvent("GameCreated", 
                tags: new ActivityTagsCollection() 
                { 
                    new KeyValuePair<string, object?>("gameType", gameType) 
                }));
    }
}
