using Codebreaker.Data.SqlServer;
using Codebreaker.SqlServerMigration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ApiDbInitializer>();

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(ApiDbInitializer.ActivitySourceName));

builder.AddSqlServerDbContext<GamesSqlServerContext>("CodebreakerSql");

var app = builder.Build();

app.Run();
