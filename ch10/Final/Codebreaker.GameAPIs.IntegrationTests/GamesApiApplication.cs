[assembly: AssemblyTrait("Category", "SkipWhenLiveUnitTesting")]

namespace Codebreaker.GameAPIs.Tests;

internal class GamesApiApplication(string environment = "Development", string solutionEnvironment = "Local", string datastore = "InMemory") : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostOptions(webBuilder =>
        {
        });

        builder.UseEnvironment(environment);

        builder.ConfigureServices(services =>
        {
                
        });

        Environment.SetEnvironmentVariable("DataStore", datastore);
        Environment.SetEnvironmentVariable("SolutionEnvironment", solutionEnvironment);
        return base.CreateHost(builder);
    }
}

