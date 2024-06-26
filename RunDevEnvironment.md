# Running the application with the development environment

Version May-2024

To run the application from your local developer system, you need to specify some settings. This file describes all the thinkgs that need to be configud to run the applciation:

- Using Azure Cosmos DB (starting with chapter 3)
- Using SQL Server (starting with chapter 3)
- Using Azure (starting with chapter 6)

## Using Azure Cosmos DB

Starting with chapter 3, we use Azure Cosmos DB.

To use Azure Cosmos DB, it's best to use the local Azure Cosmos DB emulator (as of now), or running this service with Microsoft Azure (starting with chapter 6).

### Using the Azure Cosmos DB Emulator on Windows

Install the Cosmos emulator: 

`winget install Microsoft.Azure.CosmosEmulator` 

This app model configuration gets the connection string 

    var cosmos = builder.AddConnectionString("codebreakercosmos");

With appsettings.Development.json, configure the DataStore, and set ConnectionStrings for the emulator:

Config

```json
  "DataStore": "Cosmos",
  "ConnectionStrings": {
    "GamesCosmosConnection": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;"
  }
```

### Azure Cosmos DB emulator running with Docker    

The Docker emulator currently has some issues that are in progress to be fixed:
https://github.com/dotnet/aspire/discussions/2535^
https://github.com/Azure/azure-cosmos-dotnet-v3/issues/4315#issuecomment-1986522226

## Using SQL Server

Starting with chapter 3, we use SQL Server.

### Using the Docker container to run SQL Server:

Codebreaker.AppHost/Programc.cs
```csharp
    var sqlServer = builder.AddSqlServer("sql")
        .WithDataVolume()
        .AddDatabase("CodebreakerSql", "codebreaker");
```

Running locally, this creates a Docker container with a volume that's persisted, so all your games are stored between invocations. You need to configure the password `Parameters:sql-password' with the user secrets, otherwise a random password is generated which is not the same between invocations, and login fails!

Configure this use-secrets:

`dotnet user-secrets set "Parameters:sql-password" "Passw0rd1!"`

Running the application, check the environment variables with the .NET Aspire dashboard to see the password with SQL and the game APIs service, and if they match. Check the log of SQL Server if the login of sa fails. If this is the case, delete the Docker volume (the previous stored games will be lost) and restart again.

## Using Azure

Starting the application on your local system, configure these settings with the user secrets of the AppHost project:

```bash
dotnet user-secrets set "Azure:SubscriptionId" "your subscription id"
dotnet user-secrets set "Azure:Location" "your prefered Azure region"
dotnet user-secrets set "Azure:CredentialSource" "AzureCli"
```

To get to your subscription id:

`az account show --query id`

To list available regions:

`az account list-locations --query "[].{Region:name}" -o table`

Depending on your configuration, setting the credentials might not be required. By default, `DefaultAzureCredential` is used which uses EnvironmentCredential, WorkloadIdentityCredential, ManagedIdentityCredential, SharedTokenCacheCredential, VisualStudioCredential, VisualStudioCodeCredential, AzureCliCredential, AzurePowerShellCredential, AzureDeveloperCliCredential, and InteractiveBrowserCredential, There might be a wrong account or wrong subscription be scuccessull with a login. If you set the credential configuration to AzureCli, the credentials you are loggged into with the Azure CLI are used.


> Because every chapter uses the same user secrets id with the AppHost project, the configuration needs to be done only once for all chapters.
 