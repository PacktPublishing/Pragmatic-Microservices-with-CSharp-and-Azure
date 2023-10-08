using System.Collections;

Console.WriteLine("Defined environment variables");
IDictionary vars = Environment.GetEnvironmentVariables();
foreach (object? key in vars.Keys)
{
    Console.WriteLine($"{key} {vars[key]}");
}
Console.WriteLine();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("connectionstrings.json", optional: true);

string appConfigConnectionString = builder.Configuration["AzureAppConfigurationConnection"] ?? throw new InvalidOperationException("AzureAppConfigurationConnection not found");
builder.Configuration.AddAzureAppConfiguration(appConfigConnectionString);

// Add services to the container.

builder.Services.Configure<Service1Options>(builder.Configuration.GetSection("Service1"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/readconfig", (IConfiguration config) =>
{
    string? config1 = config["Config1"];
    return $"config1: {config1}";
});

app.MapGet("/readoptions", (IOptions<Service1Options> options) =>
{
    return $"options - config1: {options.Value.Config1}; config 2: {options.Value.Config2}";
});

app.MapGet("/azureconfig", (IConfiguration config) =>
{
    string? connectionString = config.GetSection("ConfigurationPrototype").GetConnectionString("SqlServer");
    return $"Configuration value from Azure App Configuration: {connectionString}";
});

app.Run();

internal class Service1Options
{
    public required string Config1 { get; set; }
    public string? Config2 { get; set; }
}