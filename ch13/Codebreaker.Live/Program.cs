using Codebreaker.Live;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapApplicationEndpoints(app.Logger);

// Swagger currently has an issue with TimeSpan; I expect this is solved latest with .NET 9
// https://github.com/dotnet/aspnetcore/issues/54526

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
