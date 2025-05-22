# Chapter 3 - Writing data to relational and NoSQL databases

## Technical requirements

### Source code repo

The code for this chapter can be found in the following GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure

The source code folder ch03 contains the code samples for this chapter. You'll find the code for the four core projects:

* `Codebreaker.Data.SqlServer` - this is the new library to access Microsoft SQL Server.
* `Codebreaker.Data.Cosmos` - this is the new library to access Azure Cosmos DB.
* `Codebreaker.Data.Postgres` - this is a relational alternative to SQL Server (e.g. when you use an ARM CPU).
* `Codebreaker.GamesAPIs` - this is the Web API project created in the previous chapter. In this chapter the dependency injection container to use the repository implementations 
* These projects are used in this chapter but unchanged from the previous chapter: `Codebreaker.AppHost`, `Codebreaker.ServiceDefaults`, and `Codebreaker.GameAPIs.Models`.

The games analyzer library from the previous chapter is not included with this chapter. Here we'll just use the NuGet package CNinnovation.Codebreaker.Analyzers.

> If you worked through the previous chapter to create the models and implemented the minimal API project, you can continue from there.  You can also copy the files from the folder ch02 if you didn't complete the previous work, and start from there. Ch03 contains all the updates from this chapter.

### Development tools used

Other than a development environment, you need Microsoft SQL Server and Azure Cosmos DB.

You can use Docker Desktop to run Azure Cosmos DB, SQL Server, and PostreSQL in a Docker container.

See [Installation of SQL Server, Azure Cosmos DB](../installation.md)

## Running the application in the development environment

See [Running the application with the development environment](../RunDevEnvironment.md) how to use Azure Cosmos DB and SQL Server

### In-Memory

Set this setting with `appsettings.json` (project `Codebreaker.AppHost`):

```json
{
  "CodebreakerSettings": {
    "DataStore": "InMemory",
  }
}
```

### SQL Server

Set the `DataStore` and the `GamesSqlServerConnection with `appsettings.Development.json`

```json
{
  "CodebreakerSettings": {
    "DataStore": "SqlServer",
  }
}
```

When you start the application with SQL Server running in a Docker container, the password will be automatically set. You can check the password reading *user secrets*. To read these from the command line, use:

```bash
cd Codebreaker.AppHost
dotnet user-secrets list
```

You can also configure the password using:

```bash
cd Codebreaker.AppHost
dotnet user-secrets set Parameters:sql-password Passw0rd!Passw0rd
```

Be aware that once you set the password, it's configured with the Docker volume named *Codebreaker.AppHost-sql-data*. To delete the volume, and start fresh, you can delete the volume:

```bash
docker volume rm Codebreaker.AppHost-sql-data
```

Database migration happens when starting the application, thus the database is created on first use with the `SqlServer` setting (see the `ApplicationServices` class, in the method `CreateOrUpdateDatabaseAsync`).

### Azure Cosmos DB

> The preview version of the Docker image for Cosmos is a lot better than the released version. It's just not complete with its APIs yet, thus some of the queries are not working (yet).

As an alternative, to test the application running Azure Cosmos DB, run the Azure Cosmos DB emulator locally:

Install the Cosmos emulator: `winget install Microsoft.Azure.CosmosEmulator` and create the **codebreaker** database within this emulator.

In case there are previous installations of the Cosmos emulator which stopped working, reset the data, or if this fails, uninstall the emulator and install it again.

Running the application with Azure Cosmos DB, set the `DataStore` with `appsettings.Development.json` (project Codebreaker.AppHost), and set the `GamesCosmosConnection` (with a well-known endpoint to the emulator):

```json
  "DataStore": "Cosmos",
  "ConnectionStrings": {
    "GamesCosmosConnection": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;"
  }
```

Make sure the app model in the App Host has the correct configuration - either using the local running emulator, or the Docker container if the container is running.

The database is created on startup of the application (see the `ApplicationServices` class, in the method `CreateOrUpdateDatabaseAsync`).

## Codebreaker diagrams

* [Components diagram](components.drawio)
* [Create a game using SQL Server](CreateAGameWithSQLServer.md)

## Updates

Using `dotnet ef migrations` fails referencing the games API startup project because the EF Core configuration is now in `ApplicationServices` instead of the `Program.cs`, and thus cannot be found from the `dotnet ef` tool.

See the different options to create the context at design time: [Design-time DbContext Creation](https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation)

I added the `GamesSqlServerContextFactory` class to the SQL Server library which can be used this way to create a new migration:

```bash
cd Codebreaker.Data.SqlServer
dotnet ef migrations add <Name> -- "server=(localdb)\mssqllocaldb;database=Test;trusted_connection=true"
```

A connection string to the database needs to be passed following `--` which is read with arguments from the `GamesSqlServerContextFactory`.
