using Aspire.Hosting.Azure;

using Azure.Provisioning.EventHubs;

namespace Codebreaker.AppHost;

internal static partial class DistributedApplicationBuilderExtensions
{
    public static IResourceBuilder<AzureEventHubResource> AddCodebreakerEventHub(this IDistributedApplicationBuilder builder, bool useEmulator)
    {
        var eventHubs = builder.AddAzureEventHubs("events")
             .ConfigureInfrastructure(infra =>
             {
                 var eventHubs = infra.GetProvisionableResources()
                                      .OfType<EventHubsNamespace>()
                                      .Single();

                 eventHubs.Sku = new EventHubsSku()
                 {
                     Name = EventHubsSkuName.Standard,
                     Tier = EventHubsSkuTier.Standard,
                     Capacity = 1,
                 };                 
                 eventHubs.PublicNetworkAccess = EventHubsPublicNetworkAccess.Enabled;
                 eventHubs.Tags.Add("solution", "codebreaker");
             });

        if (useEmulator)
        {
            eventHubs.RunAsEmulator();
        }
        var eventHub = eventHubs.AddHub("completedgames");
        
        return eventHub;        
    }
}
