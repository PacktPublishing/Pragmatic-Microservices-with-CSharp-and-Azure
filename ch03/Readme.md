# Chapter 3

## Technical requirements

The code for this chapter can be found in the following GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure

The source code folder ch03 contains the code samples for this chapter. You’ll find the code for the four core projects:

* Codebreaker.Data.SqlServer – this is the new library to access Microsoft SQL Server.
* Codebreaker.Data.Cosmos – this is the new library to access Azure Cosmos DB.
* Codebreaker.GamesAPIs – this is the Web API project created in the previous chapter. In this chapter the dependency injection container to use the repository implementations 
* Codebreaker.GamesAPIs.Models – a library for the data models. This is unchanged from the previous chapter.

The algorithms libraries from the previous chapter are not included with this chapter. Here we’ll just use the NuGet package CNinnovation.Codebreaker.Analyzers.

If you worked through the previous chapter to create the models and implemented the minimal API project, you can continue from there.  You can also copy the files from the folder ch02 if you didn’t complete the previous work, and start from there. Ch03 contains all the updates from this chapter.

Other than a development environment, you need Microsoft SQL Server and Azure Cosmos DB. You don’t need an Azure subscription at this point. SQL Server is installed together with Visual Studio. You can also download the SQL Server 2022 Developer edition instead. This is easy via winget:

`winget install Microsoft.SQLServer.2022.Developer`

To read and write your SQL Server data, within Visual Studio you can use the SQL Server Object Explorer. Outside of Visual Studio, and with more functionality, use SQL Server Management Studio which can be installed with:

`winget install Microsoft.SQLServerManagementStudio`

To access Azure Cosmos DB, an emulator to run it locally is available. You can install this NoSQL database emulator with:

`winget install Microsoft.Azure.CosmosEmulator`

## Installing via Microsoft Installer

Instead of using winget, you can also install the software using Microsoft Installers. With SQL Server, chose the Developer or Express editions.

* [Microsoft SQL Server][https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
* [Azure Cosmos DB Emulator](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21#install-the-emulator)

## Using SQL Server and Azure Cosmos DB on Linux or Mac

Using the Mac, you can use Docker to run SQL Server and Azure Cosmos DB. Read chapter 5 for more information on hosting SQL Server with Docker.

* [Run SQL Server Linux container images with Docker](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker)
* [Run the Azure Cosmos DB Linux Emulator on Docker](https://learn.microsoft.com/en-us/azure/cosmos-db/docker-emulator-linux)


## Updates after draft

The Cosmos partition key changed to be independent of `Game` objects.

The chapter will be changed.