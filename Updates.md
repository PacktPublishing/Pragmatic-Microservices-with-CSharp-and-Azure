# .NET Aspire and book updates

What's changed with the new versions of .NET Aspire? What else changed with the book?

## Overall changes

### Central Package Management (CPM)

With all the chapters (with the exception of chapter 1) we now use **NuGet Central Package Management (CPM)** with package versions specified in the file *Directory.Packages.props'. This makes it easier to update all chapters.
In case you copy the content of just a single chapter, also copy the file *Directory.Packages.props* from the root folder to get all the projects compiled.

### Easier configuration

The book sample allows to be deployed to Microsoft Azure, or On-premises, using different services as needed. I've changed the configuration to make this easier, to allow configuring this on a service by service base. For various names, I've changed the code to use constants for easier consistency.

The ServiceDefaults project now contains these files:

- `CodebreakerSettings.cs`: contains the enum values for the configuration settings, and the `CodebreakerSettings` class which is filled reading the configuration.
- `EnvVarNames.cs` - environment variables that are passed to service projects
- `ServiceNames.cs` - const values for service names 

This project is already referenced by the service projects.
To reference it from the AppHost project to use the configuration with the app-model, the project reference needs this setting:
`IsAspireProjectResource="false"`.

All other project references are triggered to create manifest code for strongly-typed references.

```xml
    <ProjectReference Include="..\Codebreaker.ServiceDefaults\Codebreaker.ServiceDefaults.csproj" IsAspireProjectResource="false" />
```

## .NET Aspire 9.0 - 9.4 Updates

### Chapter 1, Introduction to .NET Aspire and Microservices

#### Page 4, Starting with .NET Aspire:

The *Aspire workload* is no longer required with Aspire 9. Instead, the Aspire SDK is specified with the AppHost project file:

```xml
 <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />
```

The `IsAspireHost` property is no longer required in the project file. This property was moved to `Aspire.AppHost.Sdk`.

https://github.com/dotnet/docs-aspire/blob/main/docs/whats-new/dotnet-aspire-9.2.md#-project-file-changes

#### Page 6, The .NET Aspire app model

Health checks are added to the app model:

```csharp
var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice")
    .WithHttpsHealthCheck("/health");

builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpsHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);
```

#### Page 7, The app model with the generated code:

```csharp
builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);
```

The `WaitFor` method is new with .NET Aspire 9 which allows waiting with the start of the webfrontend until the `apiService` has been started and is healthy.

#### Page 8, The shared project for common configuration

The signature for the extension methods in the generated **ServiceDefaults** library changed the signature.

This is the original version:

```csharp
public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBBuilder builder)
{
}
```

This method is now generic:

```csharp
public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
{
}
```

With this update, also the methods `ConfigureOpenTelemetry`, `AddOpenTelemetryExporters` and `AddDefaultHealthChecks` are now generic. Changing these methods to their generic version does not break the existing calling code. This just gives more flexibility with other builder types. 
See also https://github.com/PacktPublishing/Pragmatic-Microservices-with-CSharp-and-Azure/discussions/234.

#### Page 11, Aspire/AspireSample.AppHost/Program.cs

With the .NET Aspire 9.3 templates, the `Program.cs` file has been renamed to `AppHost.cs`.

See https://github.com/dotnet/aspire/issues/8681

#### Page 13, .NET Aspire integrations

*.NET Aspire components* have been renamed to *.NET Aspire integrations*

### Chapter 2, Minimal APIs - Creating REST Services

#### Page 54, Testing the service
HTTP Files support setting variables to access the result from an invocation, and use it with a next request, e.g.

```json
### Create a game
# @name create
POST {{Codebreaker.GameAPIs_HostAddress}}/games/
Content-Type: {{ContentType}}

{
  "gameType": "Game6x4",
  "playerName": "test"
}

### Set a move
PATCH {{Codebreaker.GameAPIs_HostAddress}}/games/{{create.response.body.$.id}}
Content-Type: {{ContentType}}

{
  "gameType": "Game6x4",
  "playerName": "test",
  "moveNumber": 1,
  "guessPegs": [
    "Red",
    "Green",
    "Blue",
    "Yellow"
  ]
}
```

Using `@name create` specifies the `create` variable, and the result is accessed using `{{create.response.body.$.id}}`.

#### Page 57, Exploring the ServiceDefaults library

The method `AddServiceDefaults` is now generic:

```csharp
public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
{
}
```

### Chapter 3, Writing Data to Relational and NoSQL Databases

Start Docker Desktop (or Podman) to use different databases not installed on the local system. The chapter has been enhanced to include *PostgreSQL*, and *Azure Cosmos DB with a Docker preview image*. This preview image requires to use the 

#### Page 78, Codebreaker.AppHost/Program.cs

You no longer need to reference the SQL Server password. You can use this code:

```csharp
    var sqlDB = builder.AddSqlServer("sql")
        .WithDataVolume("codebreaker-sql-data")
        .AddDatabase("CodebreakerSql", "codebreaker");
```

With this, the password is stored in the user secrets. Check the user secrets to read it.

#### Page 93

A Docker image is used here to run Azure Cosmos DB locally. The Docker volume `codebreaker-cosmos-data` keeps the data available between runs.

```csharp
// Codebreaker.AppHost/Program.cs
cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
  .RunAsPreviewEmulator(p =>
    p.WithDataExplorer()
    .WithDataVolume("codebreaker-cosmos-data")
    .WithLifetime(ContainerLifetime.Session));
```

Aspire .NET 9.1 allows creating the Cosmos container - using `AddCosmosDatabase` and `AddContainer` instead of `AddDatabase`.

```csharp
// Codebreaker.AppHost/Program.cs
        var cosmosDB = cosmos
            .AddCosmosDatabase("codebreaker")
            .AddContainer("GamesV3", "/PartitionKey");
```

#### New feature: PostgreSQL

Another database provider is offered. You can use PostgreSQL to store games and moves.

```csharp
var postgres = builder.AddPostgres("postgres")
  .WithDataVolume("codebreaker-postgres-data")
  .WithPgAdmin(r =>
  {
    r.WithImageTag("latest");
    r.WithImagePullPolicy(ImagePullPolicy.Always);
    r.WithUrlForEndpoint("http", u => u.DisplayText = "PG Admin");
  })
  .AddDatabase("CodebreakerPostgres");

gameApis
  .WithReference(postgres)
  .WaitFor(postgres);
```

### Chapter 5, Containerization of Microservices

Because of the change to use *Central Package Management (CPM)*, the `Dockerfile` changed to copy the file `Directory.Packages.props` for a build.

#### Page 132, Codebreaker.GameAPIs/Dockerfile

```Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["ch05/FinalDocker/Codebreaker.GameAPIs/Codebreaker.GameAPIs.csproj", "ch05/FinalDocker/Codebreaker.GameAPIs/"]
RUN dotnet restore "./ch05/FinalDocker/Codebreaker.GameAPIs/Codebreaker.GameAPIs.csproj"
WORKDIR "/src/ch05/FinalDocker/Codebreaker.GameAPIs"
COPY ["ch05/FinalDocker/Codebreaker.GameAPIs/", "."]
RUN dotnet build "./Codebreaker.GameAPIs.csproj" -c $BUILD_CONFIGURATION -o /app/build
```

#### Page 134, Building a Docker Image

The build command needs to use the context of the root directory where the file `Directory.Packages.props` is located:

```bash
cd ch05/FinalDocker
docker build ../.. -f Codebreaker.GameAPIs/Dockerfile -t codebreaker/gamesapi:3.5.4 -t codebreaker/gamesapi.latest
```

#### Page 139, Configuring a Docker container for SQL Server

It's no longer required (you still can do it) to configure a password for the SQL Server Docker container. If you don't configure one, it's created and filled automatically with user secrets. Thus, this code can be used:

```csharp
// Codebreaker.AppHost/Program.cs
var sqlDB = builder.AddSqlServer(SqlResourceName)
  .WithDataVolume(SqlDataVolume)
  .AddDatabase(SqlDatabaseResourceName, SqlDatabaseName);
```

#### Extension - Using Docker Compose with .NET Aspire

This is a new feature available since .NET Aspire 9.3):

After adding the NuGet package `Aspire.Hosting.Docker` (currently in preview with .NET Aspire 9.4), you can use the `AddDockerComposePublisher` method to publish the application as a Docker Compose file:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("docker-compose");
```

With this, you can use the `aspire publish` command to create the Docker Compose file.

### Chapter 6 - Microsoft Azure for Hosting Applications

With .NET Aspire 9.4 and preview libraries, you can use and configure environments, e.g. `builder.AddAzureContainerAppEnvironment`.
Expected with a future version the Aspire CLI can be used to deploy this environment (see `aspire deploy`).

Currently, use the Azure Developer CLI (`azd`) as described in the book to deploy the application to Azure Container Apps.

### Chapter 8 - CI/CD - Publishing with GitHub Actions

#### Page 212, Preparing the solution using the Azure Developer CLI

`azd` version 1.15.1 detects the **.NET (Aspire)** instead of **Azure Container Apps**. Similar like before, Azure Container Apps are deployed.

### Chapter 9 - Authentication and Authorization with Services and Clients

#### Page 249, Creating an Azure AD B2C tenant

At the time of the release of the book, Microsoft Entra didn't support allow users to create accounts with *Microsoft Entry External Identities*. This feature is now available.

Thus, instead of using *Azure AD B2C*, now *Microsoft Entra External Identities* can be used.

#### Keycloak

The sample application offers a new way for authentication: Keycloak.


### Chapter 10 - All about testing the solution

#### Page 279, Exploring the games analyzer library

This is an error in the book:

The filename for the source code is not:

`Codebreaker.GameAPIs.Analyzers.Tests/Analyzers/GameGuessAnalyzer.cs`

This should be:

`Codebreaker.GameAPIs.Analyzers/Analyzers/GameGuessAnalyzer.cs`

#### Page 281, Creating a unit test project

The book shows using xUnit. There are enhancements with the new **Microsoft Testing Platform**, supported by **xUnit.v3**!

To use the new **Microsoft Testing Platform** with xUnit, install the new xUnit.v3 templates:

```bash
dotnet new install xunit.v3.templates
```

Then use these commands:

```bash
dotnet new xunit3 -o Codebreaker.Analyzers.Tests
cd Codebreaker.Analyzers.Tests
dotnet reference add ..\Codebreaker.Analyzers
```

With the project file, enable these settings with the project file within the `<PropertyGroup>` element:

```xml
	<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
	<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
```

#### Page 284, Passing data to unit tests

Instead of using `IEnumerable<object[]>`, xUnit.3 supports a strongly-typed version using `TheoryDataRow`. This is the strongly typed source code (the older version still compiles and runs):

```csharp
public class TestData6x4 : IEnumerable<TheoryDataRow<string[], string[], ColorResult>>
{
    public static readonly string[] Colors6 = [Red, Green, Blue, Yellow, Black, White];

    public IEnumerator<TheoryDataRow<string[], string[], ColorResult>> GetEnumerator()
    {
        yield return new TheoryDataRow<string[], string[], ColorResult>(
            [Green, Blue,  Green, Yellow], // code
            [Green, Green, Black, White],  // input-data
            new ColorResult(1, 1) // expected
        );
        yield return new TheoryDataRow<string[], string[], ColorResult>(
            [Red,   Blue,  Black, White],
            [Black, Black, Red,   Yellow],
            new ColorResult(0, 2)
        );
// code remove for brevity
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

#### Page 289, Running unit tests

The command `dotnet test` is still working, and the *Test Explorer* is working as well. 
With the new **Microsoft Testing Platform**, you can also do a simple `dotnet run` to run the tests instead!

### Chapter 11, Logging and Monitoring

With the `Activity` class, recording exceptions has been renamed from `RecordException` to `AddException`.

Grafana config:

provisioning/datasources/default.yaml

url: $PROMETHEUS_ENDPOINT instead of using a fixed port
