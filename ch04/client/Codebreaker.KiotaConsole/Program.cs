var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RunnerOptions>(options =>
{
    options.GamesApiUrl = builder.Configuration["GamesApiUrl"] ?? throw new InvalidOperationException("GamesApiUrl not found");
});

builder.Services.AddTransient<Runner>();
var app = builder.Build();

var runner = app.Services.GetRequiredService<Runner>();
await runner.RunAsync();
