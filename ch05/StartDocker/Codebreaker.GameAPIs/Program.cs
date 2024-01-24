using System.Runtime.CompilerServices;

using Codebreaker.GameAPIs;

using Microsoft.OpenApi.Models;

[assembly: InternalsVisibleTo("Codbreaker.APIs.Tests")]

var builder = WebApplication.CreateBuilder(args);

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
});

// Application Services

builder.AddApplicationServices();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v3/swagger.json", "v3");
});

if (builder.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<GamesSqlServerContext>();
        if (repo is GamesSqlServerContext context)
        {
            await context.Database.MigrateAsync();
            app.Logger.LogInformation("Database updated");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error updating database");
    }
}

app.MapGameEndpoints();

app.Run();
