# Installation

May-2024

## Visual Studio

To install Visual Studio: `winget install Microsoft.VisualStudio.2022.Community.Preview`

[Visual Studio 2022 Download](https://visualstudio.microsoft.com/downloads/)

## Visual Studio Code

Install Visual Studio Code: `winget install Microsoft.VisualStudioCode`

## Docker desktop

Install Docker Desktop on Windows: `winget install Docker.DockerDesktop`

## Aspire

Install .NET Aspire (.NET CLI):

- dotnet workload update
- dotnet workload install aspire

You can install .NET Aspire via Visual Studio 2022 Preview as well.

See details:
https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=dotnet-cli

### .NET Aspire Daily Build

With a few chapters, the code is based on daily Aspire builds. For these, the daily build is reuired to compile and run the code successfully.

[Install daily builds .NET Aspire](https://github.com/dotnet/aspire/blob/main/docs/using-latest-daily.md)

> If possible, don't install the .NET Aspire daily build. There are some issues with uninstalling the workload. Soon all chapters will be updated to .NET Aspire Preview 7, and after this to the GA version of .NET Aspire.

Using these instructions, install the .NET Aspire dotnet workload. To retrieve the NuGet packages, a nuget.config file is used.

To remove Aspire daily builds, check [Downgrading dotnet aspire](https://github.com/dotnet/aspire/discussions/2829)

## Azure Developer CLI

Install the Azure Developer CLI at least with version **azd version 1.9.100**:

winget install Microsoft.Azd

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

