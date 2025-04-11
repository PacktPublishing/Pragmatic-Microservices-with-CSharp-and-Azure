# .NET Aspire updates

What's changed with the new versions of .NET Aspire?

With all the chapters (with the exception of chapter 1) we now use **NuGet Central Package Management (CPM)** with package versions specified in the file *Directory.Packages.props'. This makes it easier to update all chapters.
In case you copy the content of just a single chapter, also copy the file *Directory.Packages.props* from the root folder to get all the projects compiled.

## .NET Aspire 9.2 Updates

The `IsAspireHost` property is no longer required in the project file. This property was moved to `Aspire.AppHost.Sdk`.

https://github.com/dotnet/docs-aspire/blob/main/docs/whats-new/dotnet-aspire-9.2.md#-project-file-changes

### Chapter 1, Introdution to .NET Aspire and Microservices

## .NET Aspire 9.1 Updates

Azure Cosmos DB:

App-Model: `AddDatabase` --> `AddCosmosDatabase`

Azure Event Hub

App-Model: `AddEventHub` --> `AddHub`

## .NET Aspire 9.0 Updates

### Chapter 1, Introdution to .NET Aspire and Microservices

#### Page 4, Starting with .NET Aspire:

The *Aspire workload* is no longer required with Aspire 9. Instead, the Aspire SDK is specified with the AppHost project file:

```xml
 <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />
```

#### Page 7, The app model with the generated code:

```csharp
builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);
```

The WaitFor method is new with .NET Aspire 9 which allows waiting with the start of the webfrontend until the apiService has been started.

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

#### Page 13, .NET Aspire integrations

*.NET Aspire components* have been renamed to *.NET Aspire integrations*

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

```csharp
// Codebreaker.AppHost/Program.cs
        cosmos = builder.AddAzureCosmosDB("codebreakercosmos")
            .RunAsPreviewEmulator(p =>
                p.WithDataExplorer()
                .WithDataVolume("codebreaker-cosmos-data")
                .WithLifetime(ContainerLifetime.Session));
```

A Docker image is used here to run Azure Cosmos DB locally. The Docker volume `codebreaker-cosmos-data` keeps the data available between runs.

```csharp
// Codebreaker.AppHost/Program.cs
        var cosmosDB = cosmos
            .AddCosmosDatabase("codebreaker")
            .AddContainer("GamesV3", "/PartitionKey");
```

Aspire .NET 9.1 allows creating the Cosmos container - using `AddCosmosDatabase` and `AddContainer` instead of `AddDatabase`.

### Chapter 5, Containerization of Microservices

Because of the change to use *Central Package Management (CPM)*, the Dockerfile changed to copy the file *Directory.PAckages.props* for a build.

#### Page 132, Codebreaker.GameAPIs/Dockerfile

```Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["ch05/FinalDocker/Codebreaker.GameAPIs/Codebreaker.GameAPIs.csproj", "ch05/FinalDocker/Codebreaker.GameAPIs/"]
RUN dotnet restore "./ch05/FinalDocker/Codebreaker.GameAPIs/Codebreaker.GameAPIs.csproj"
COPY . .
WORKDIR "/src/ch05/FinalDocker/Codebreaker.GameAPIs"
RUN dotnet build "./Codebreaker.GameAPIs.csproj" -c $BUILD_CONFIGURATION -o /app/build
```

### Page 134, Building a Docker Image

The build command needs to use the context of the root directory where the file `Directory.Packages.props` is located:

```bash
docker build ../.. -f Codebreaker.GameAPIs/Dockerfile -t codebreaker/gamesapi:3.5.4 -t codebreaker/gamesapi.latest
```

### Chapter 11, Logging and Monitoring

With the `Activity` class, recording exceptions has been renamed from `RecordException` to `AddException`.
