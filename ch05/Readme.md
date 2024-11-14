# Chapter 5 - Containerization of Microservices

## Technical requirements

What you need to go through this chapter is **Docker Desktop**. *Docker Desktop is free for individual developers, education and open source communities. You can download Docker Desktop from [Docker Desktop](https://www.docker.com/products/docker-desktop/).

### Install WSL 2

To install the Windows Subsystem for Linux, read [How to install Linux on Windows with WSL](https://learn.microsoft.com/en-us/windows/wsl)

### Install Docker Desktop

To install Docker Desktop, read [Install Docker Desktop on Windows](https://docs.docker.com/docker-for-windows/install/)

### Source Code

The code for this chapter can be found in the following GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure.

The source code folder ch05 contains the code samples for this chapter. For different sections of this chapter, different subfolders are available. For a start, working though the instructions, you can use the *StartXX* folders. *StartDocker* contains the projects before creating Docker containers have been added, the folder *FinalDocker* contains the project in the final state after building the Docker container. 
The *StartAspire* folder contains multiple projects where the .NET Aspire specific projects that we created in the previous chapters are already part of. Use this as a starting point to work through the .NET Aspire part of this chapter. *FinalAspire* contains the complete result that you can use as reference. The folder *NativeAOT* contains the code for the games API that compiles with .NET native AOT.

In the subfolders of the ch05 folder, you’ll see these projects:

* Codebreaker.GameAPIs – the games API project we used in the previous chapter from our client application. In this chapter we make minor updates to specify the connection string to the SQL Server database. This project has a reference to NuGet packages with implementations of the IGamesRepository interface for SQL Server and Azure Cosmos DB.
* Codebreaker.Bot - this is the new project that implements a REST API and calls the games API to automatically play games with random game moves. This project makes use of the client-library we created in Chapter 4 – it has a reference to the NuGet package CNinnovation.Codebreaker.Client to call the games API.
* Codebreaker.AppHost – this project is enhanced to orchestrate the different services.
* Codebreaker.ServiceDefaults – this project is unchanged in this chapter.
* Codebreaker.GameAPIs.NativeAOT – a new project which offers the same games API with some changes to support native AOT with .NET 9

## Running the application locally with a SQL Server Docker container

See [Running in the Dev Environment](../RunDevEnvironment.md)

Using SQL Server, you can define the app-model without using a password:

```csharp
var sqlServer = builder.AddSqlServer("sql") //, sqlPassword)
    .WithDataVolume("codebreaker-sql-data", isReadOnly: false)
    .AddDatabase("CodebreakerSql");
```

Because of the `WithDataVolume` method, a data volume will be created and mapped, and a random password is generated. This password is stored with user secrets.

To specify a default password, create this configuration best with user secrets:

```json
  "Parameters:sql-password": "Password123!"
```

User secrets can be specified using Visual Studio (*Manage User Secrets*), or with the .NET CLI:

`bash
dotnet user-secrets set Parameters:sql-password "Password123!"
`

`sql-password` needs to match the name passed with `AddSqlServer`, and `-password` suffixed. Remember to delete the `codebreaker-sql-data` volume if this exists with the previous password.

Another option is to retrieve the password as parameter, and assign it to `AddSqlServer`:

```csharp
var sqlPassword = builder.AddParameter("SqlPassword", secret: true);

var sqlServer = builder.AddSqlServer("sql", sqlPassword)
    .WithDataVolume("codebreaker-sql-data", isReadOnly: false)
    .AddDatabase("CodebreakerSql");
```

This overwrites the default password and retrieves it from parameters with the specified name:

```json
  "Parameters:SqlPassword", "Another123!"
```

The command to specify this parameter with user secrets:

```bash
dotnet user-secrets set Parameters:SqlPassword "AnotherPassw0rd123!"
```

Verify if this is successful, using the Aspire dashboard, and checking *Details* of the sql container, environment variable `MSSQL_SA_PASSWORD', and the connection string retrieved from the *gameapis* project `ConnectionStrings__Codebreaker_Sql`. The password will be visible as well.

## Codebreaker diagrams

[Docker containers](containerdiagram.md)
