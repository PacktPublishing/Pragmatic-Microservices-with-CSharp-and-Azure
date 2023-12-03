var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages(); // authentication UI in Views

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();
app.MapReverseProxy();

app.MapGet("/", () => "Hello World!");

app.Run();
