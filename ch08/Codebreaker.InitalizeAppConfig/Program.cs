using Codebreaker.InitalizeAppConfig;
using Microsoft.Extensions.Azure;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<AppConfigInitializer>();
builder.Services.AddAzureClients(clients =>
{
    string appConfigUrl = builder.Configuration.GetConnectionString("codebreakerconfig") ?? throw new InvalidOperationException("codebreakerconfig not configured");
    clients.AddConfigurationClient(new Uri(appConfigUrl));
});

var host = builder.Build();
host.Run();
