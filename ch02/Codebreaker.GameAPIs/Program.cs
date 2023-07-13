using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

using Codebreaker.GameAPIs.Data;
using Codebreaker.GameAPIs.Data.InMemory;

using Microsoft.OpenApi.Models;

[assembly: InternalsVisibleTo("Codbreaker.APIs.Tests")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter<GameType>());
});

// Swagger/EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v3", new OpenApiInfo
    {
        Version = "v3",
        Title = "Codebreaker Games API",
        Description = "An ASP.NET Core minimal API to play Codebreaker games",
        TermsOfService = new Uri("https://www.cninnovation.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Christian Nagel",
            Url = new Uri("https://csharp.christiannagel.com")
        },
        License = new OpenApiLicense
        {
            Name="License API Usage",
            Url= new Uri("https://www.cninnovation.com/apiusage")
        }
    });

    options.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);
    // options.UseOneOfForPolymorphism();
});
builder.Services.AddProblemDetails();

// Application Services

builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
builder.Services.AddScoped<IGamesService, GamesService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // options.InjectStylesheet("/swagger-ui/swaggerstyle.css");
        options.SwaggerEndpoint("/swagger/v3/swagger.json", "v3");
    });
}

// -------------------------
// Endpoints
// -------------------------

app.MapGameEndpoints();

app.Run();
