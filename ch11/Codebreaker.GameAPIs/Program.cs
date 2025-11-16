using Codebreaker.GameAPIs;

using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Debug: Log all OpenTelemetry related environment variables
Console.WriteLine("=== OpenTelemetry Configuration ===");
Console.WriteLine($"OTEL_EXPORTER_OTLP_ENDPOINT (config): {builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "(not set)"}");
Console.WriteLine($"OTEL_EXPORTER_OTLP_ENDPOINT (env): {Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") ?? "(not set)"}");
Console.WriteLine($"OTEL_EXPORTER_OTLP_PROTOCOL (config): {builder.Configuration["OTEL_EXPORTER_OTLP_PROTOCOL"] ?? "(not set)"}");
Console.WriteLine($"OTEL_EXPORTER_OTLP_PROTOCOL (env): {Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL") ?? "(not set)"}");
Console.WriteLine($"OTEL_SERVICE_NAME (config): {builder.Configuration["OTEL_SERVICE_NAME"] ?? "(not set)"}");
Console.WriteLine($"OTEL_SERVICE_NAME (env): {Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ?? "(not set)"}");
Console.WriteLine("====================================");

// Test DNS resolution for otelcollector
try
{
    var addresses = System.Net.Dns.GetHostAddresses("otelcollector");
    Console.WriteLine($"DNS Resolution for 'otelcollector': {string.Join(", ", addresses.Select(a => a.ToString()))}");
}
catch (Exception ex)
{
    Console.WriteLine($"DNS Resolution for 'otelcollector' FAILED: {ex.Message}");
}

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

app.MapGameEndpoints();

await app.CreateOrUpdateDatabaseAsync();

app.Run();
