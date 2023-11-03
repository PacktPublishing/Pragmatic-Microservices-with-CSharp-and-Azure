using Microsoft.FeatureManagement;

namespace Codebreaker.GameAPIs.Extensions;

public static class FeatureManagerExtensions
{
    private static List<string>? s_featureNames;
    public static async Task<bool> IsGameTypeAvailable(this IFeatureManager featureManager, GameType gameType)
    {
        async Task<List<string>> GetFeatureNamesAsync()
        {
            List<string> featureNames = [];
            await foreach (string featureName in featureManager.GetFeatureNamesAsync())
            {
                featureNames.Add(featureName);
            }
            return featureNames;
        }

        string featureName = $"Feature{gameType}";
        if ((s_featureNames ??= await GetFeatureNamesAsync()).Contains(featureName))
        {
            return await featureManager.IsEnabledAsync(featureName);
        }
        else
        {
            return true;
        }
    }
}
