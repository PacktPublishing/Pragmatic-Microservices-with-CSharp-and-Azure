using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Codebreaker.GameAPIs.Tests;

internal class GamesApiApplication(string environment = "Development") : WebApplicationFactory<Program>
{
    private readonly string _environment = environment;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostOptions(webBuilder =>
        {
        });

        builder.UseEnvironment(_environment);

        builder.ConfigureServices(services =>
        {
                
        });

        Environment.SetEnvironmentVariable("DataStorage", "InMemory");
        return base.CreateHost(builder);
    }
}

