namespace Codebreaker.AppHost;

internal static partial class DistributedApplicationBuilderExtensions
{
    public static IResourceBuilder<RedisResource> AddCodebreakerRedis(this IDistributedApplicationBuilder builder)
    {
        var redis = builder.AddRedis("redis");
        redis.PublishAsContainer();
        redis.WithRedisCommander();
        return redis;
    }

    public static IResourceBuilder<GarnetResource> AddCodebreakerGarnet(this IDistributedApplicationBuilder builder)
    {
        var garnet = builder.AddGarnet("garnet");
        garnet.PublishAsContainer();

        return garnet;
    }
}
