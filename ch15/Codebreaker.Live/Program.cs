using Codebreaker.Live;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // work-around with Swashbuckle: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2505
    options.MapType<TimeSpan>(() => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString("00:00:00")
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapApplicationEndpoints();

// Swagger currently has an issue with TimeSpan; I expect this is solved latest with .NET 9
// https://github.com/dotnet/aspnetcore/issues/54526

app.UseSwagger();
app.UseSwaggerUI();

//var processor = app.Services.GetRequiredService<LiveGamesEventProcessor>();
//await processor.StartProcessingAsync();

app.Run();
