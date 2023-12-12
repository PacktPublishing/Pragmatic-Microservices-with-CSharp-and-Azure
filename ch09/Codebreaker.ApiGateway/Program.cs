var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages(); // authentication UI in Views

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(AzureAd, OpenIdConnectDefaults.AuthenticationScheme)
    .EnableTokenAcquisitionToCallDownstramApi().
    .AddDownstreamApi("dfjkfdj", BotApi)
    .AddInMemoryTokenCache();

var app = builder.Build();
app.MapReverseProxy();

app.MapGet("/", () => "Hello World!");

app.Run();

// https://github.com/microsoft/woodgrove-groceries