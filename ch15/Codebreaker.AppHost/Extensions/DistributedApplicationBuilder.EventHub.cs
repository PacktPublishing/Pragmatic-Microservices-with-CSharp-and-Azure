using Aspire.Hosting.Azure;

using Azure.Provisioning.EventHubs;

namespace Codebreaker.AppHost;

internal static partial class DistributedApplicationBuilderExtensions
{
    public static IResourceBuilder<AzureEventHubResource> AddCodebreakerEventHub(this IDistributedApplicationBuilder builder, bool useEmulator)
    {
        var eventHubs = builder.AddAzureEventHubs("codebreakerevents")
             .ConfigureInfrastructure(infra =>
             {
                 var eventHubs = infra.GetProvisionableResources()
                                      .OfType<EventHubsNamespace>()
                                      .Single();

                 eventHubs.Sku = new EventHubsSku()
                 {
                     Name = EventHubsSkuName.Basic,
                     Tier = EventHubsSkuTier.Basic,
                     Capacity = 1,
                 };
                 eventHubs.PublicNetworkAccess = EventHubsPublicNetworkAccess.Enabled;
                 eventHubs.Tags.Add("Solution", "Codebreaker");
             });

        if (useEmulator)
        {
            eventHubs.RunAsEmulator();
        }
        var eventHub = eventHubs.AddHub("games");

        return eventHub;        
    }
}
