using Codebreaker.Live;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.AddServiceDefaults();
builder.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.CreateOrUpdateDatabaseAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.MapApplicationEndpoints();

app.Run();
