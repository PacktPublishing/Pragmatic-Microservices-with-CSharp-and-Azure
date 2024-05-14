using Codebreaker.Data.Cosmos;
using Codebreaker.SqlServerMigration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ApiDbInitializer>();

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(ApiDbInitializer.ActivitySourceName));

builder.AddCosmosDbContext<GamesCosmosContext>(connectionName: "codebreakercosmos", databaseName: "codebreaker");

var app = builder.Build();

app.Run();
