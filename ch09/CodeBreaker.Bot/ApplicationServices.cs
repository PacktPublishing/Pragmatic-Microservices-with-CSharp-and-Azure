namespace CodeBreaker.Bot;

internal static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<GamesClient>(client =>
        {
            client.BaseAddress = new Uri("http+https://gameapis");
        });

        builder.Services.AddScoped<CodeBreakerTimer>();
        builder.Services.AddScoped<CodeBreakerGameRunner>();
    }
}
