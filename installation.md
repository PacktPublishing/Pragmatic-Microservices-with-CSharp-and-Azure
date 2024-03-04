# Installation

Mar-2024

## Visual Studio

To install Visual Studio: `winget install Microsoft.VisualStudio.2022.Community.Preview`

[Visual Studio 2022 Download](https://visualstudio.microsoft.com/downloads/)

## Visual Studio Code

Install Visual Studio Code: `winget install Microsoft.VisualStudioCode`

## Aspire

[Install daily builds .NET Aspire](https://github.com/dotnet/aspire/blob/main/docs/using-latest-daily.md)

With some chapters, this is currently required to build the code successfully.

## Docker desktop

Install Docker Desktop on Windows: `winget install Docker.DockerDesktop`

## SQL server

Starting with chapter 3, we use Microsoft SQL Server.

SQL Server comes installed together with Visual Studio.

You can also download the SQL Server Developer edition via winget:

`winget install Microsoft.SQLServer.2022.Developer`

### Installation of SQL Server via MSIX

If you don't have `winget` available on your system, you can install SQL Server by downloading the MSIX installer from [Try SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads). 

In case you use a Mac development system, you can use a Docker image for SQL Server (see more in chapter 4). You can also use SQL Azure which is described in chapter 5.

### SQL Server Management Studio

To read and write your SQL Server data, within Visual Studio you can use the SQL Server Object Explorer. Outside of Visual Studio, and with more functionality, use SQL Server Management Studio which can be installed with:

`winget install Microsoft.SQLServerManagementStudio`

### Installing SQL Server via Microsoft Installer

Instead of using winget, you can also install the software using Microsoft Installers. With SQL Server, chose the Developer or Express editions.

[Microsoft SQL Server][https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Running SQL Server in a Docker container

You can run SQL Server in a Linux Docker container:

* [Run SQL Server Linux container images with Docker](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker)

## Azure Cosmos DB

To access Azure Cosmos DB, an emulator to run it locally is available. You can install this NoSQL database emulator with:

`winget install Microsoft.Azure.CosmosEmulator`

More information on installing the Azure Cosmos DB emulator:

* [Azure Cosmos DB Emulator](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21#install-the-emulator)

### Using and Azure Cosmos DB with a Docker container

Using Azure Cosmos on non-Windows systems, you can use the Docker image for Azure Cosmos DB emulator, or use Azure Cosmos DB in the cloud.

* [Run the Azure Cosmos DB Linux Emulator on Docker](https://learn.microsoft.com/en-us/azure/cosmos-db/docker-emulator-linux)

To use Azure Cosmos DB with Microsoft Azure. Read chapter 6 for more information.

## Mirosoft Azure

