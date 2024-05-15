using Microsoft.OpenApi.Models;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMetrics();

builder.Services.AddSingleton<GamesMetrics>();

builder.Services.AddKeyedSingleton("Codebreaker.GameAPIs", (services, _) => new ActivitySource("Codebreaker.GameAPIs", "1.0.0"));  

builder.AddServiceDefaults();

// Swagger/EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v3", new OpenApiInfo
    {
        Version = "v3",
        Title = "Codebreaker Games API",
        Description = "An ASP.NET Core minimal APIs to play Codebreaker games",
        TermsOfService = new Uri("https://www.cninnovation.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Christian Nagel",
            Url = new Uri("https://csharp.christiannagel.com")
        },
        License = new OpenApiLicense
        {
            Name = "License API Usage",
            Url = new Uri("https://www.cninnovation.com/apiusage")
        }
    });
});

// Application Services

builder.AddApplicationServices();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v3/swagger.json", "v3");
});

await app.CreateOrUpdateDatabaseAsync();

app.MapGameEndpoints();

app.Run();
