namespace CodeBreaker.Bot;

internal static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // step 4 done: configure the HTTP client to use the Games API service
        builder.Services.AddHttpClient<GamesClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://gameapis");
        });

        builder.Services.AddScoped<CodeBreakerTimer>();
        builder.Services.AddScoped<CodeBreakerGameRunner>();
    }
}
