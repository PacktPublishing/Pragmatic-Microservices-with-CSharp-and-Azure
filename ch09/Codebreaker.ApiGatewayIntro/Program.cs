var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages(); // authentication UI in Views

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("authUser", builder => builder
        .RequireAuthenticatedUser());

    options.AddPolicy("group1", builder => builder
        .RequireRole("group1"));
    ;
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.MapGet("/", () => "This is the application gateway");

app.Run();
