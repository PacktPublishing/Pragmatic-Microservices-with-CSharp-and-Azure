using Aspire.Hosting.Azure;

namespace Codebreaker.AppHost;

internal static partial class DistributedApplicationBuilderExtensions
{
    public static AzureStorageReturn AddCodebreakerStorage(this IDistributedApplicationBuilder builder, bool useEmulator)
    {
        var storage = builder.AddAzureStorage("storage");

        if (useEmulator)
        {
            storage.RunAsEmulator();
        }

        var botQueue = storage.AddQueues("botqueue");
        var blob = storage.AddBlobs("checkpoints");

        return new AzureStorageReturn(storage, botQueue, blob);
    }
}

public record class AzureStorageReturn(
    IResourceBuilder<AzureStorageResource> Storage,
    IResourceBuilder<AzureQueueStorageResource> BotQueue,
    IResourceBuilder<AzureBlobStorageResource> Blob);
