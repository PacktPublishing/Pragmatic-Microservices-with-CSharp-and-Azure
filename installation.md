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

With .NET Aspire 9, installing the .NET Aspire workload is no longer required. Instead, the Aspire SDK is referenced from the AppHost project file:

```xml
  <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />
```

See details:
https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling

Visual Studio 2022 17.9 includes the .NET Aspire SDK by default.

To install the .NET Aspire templates, use 

`dotnet new install Aspire.ProjectTemplates`

See details:
https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/aspire-sdk-templates

## Azure Developer CLI

Install the Azure Developer CLI at least with version **azd version 1.11.0**:

winget install Microsoft.Azd

## SQL server

Starting with chapter 3, we use Microsoft SQL Server.

SQL Server comes installed together with Visual Studio.

You can also download the SQL Server Developer edition via winget:

`winget install Microsoft.SQLServer.2022.Developer`

> SQL Server does not run on Windows ARM64 devices such as **Surface Laptop 7 (Copilot)**. See [Developing with Visual Studio on ARM: SQL Server Challenges](https://csharp.christiannagel.com/2024/10/29/surfacewitharm/).

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

