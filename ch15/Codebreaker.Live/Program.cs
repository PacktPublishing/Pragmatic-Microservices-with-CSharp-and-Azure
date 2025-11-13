using Codebreaker.Live;

using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.AddServiceDefaults();
builder.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

app.MapDefaultEndpoints();
app.MapApplicationEndpoints();

// Swagger currently has an issue with TimeSpan; I expect this is solved latest with .NET 9
// https://github.com/dotnet/aspnetcore/issues/54526

app.UseSwagger();
app.UseSwaggerUI();

//var processor = app.Services.GetRequiredService<LiveGamesEventProcessor>();
//await processor.StartProcessingAsync();

app.Run();
