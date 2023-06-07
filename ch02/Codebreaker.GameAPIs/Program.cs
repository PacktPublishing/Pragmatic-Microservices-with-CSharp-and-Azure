using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

using Codebreaker.GameAPIs.Data;
using Codebreaker.GameAPIs.Data.InMemory;

[assembly: InternalsVisibleTo("Codbreaker.APIs.Tests")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Swagger/EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application Services

builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
builder.Services.AddScoped<IGamesService, GamesService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// -------------------------
// Endpoints
// -------------------------

app.MapGameEndpoints();

app.Run();
