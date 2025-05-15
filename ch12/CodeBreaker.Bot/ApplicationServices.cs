using System.Diagnostics;

namespace CodeBreaker.Bot;

internal static class ApplicationServices
{
    public static void AddApplicationTelemetry(this IHostApplicationBuilder builder)
    {
        const string ActivitySourceName = "Codebreaker.Bot";
        const string ActivitySourceVersion = "1.0.0";

        builder.Services.AddKeyedSingleton(ActivitySourceName, (services, _) =>
            new ActivitySource(ActivitySourceName, ActivitySourceVersion));
    }

    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<GamesClient>(client =>
        {
            client.BaseAddress = new Uri("http://gameapis");
        });

        builder.Services.AddScoped<CodeBreakerTimer>();
        builder.Services.AddScoped<CodeBreakerGameRunner>();

        builder.AddApplicationTelemetry();
    }

}
