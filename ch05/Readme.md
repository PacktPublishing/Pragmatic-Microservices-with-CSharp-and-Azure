# Chapter 5

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
* Codebreaker.GameAPIs.NativeAOT – a new project which offers the same games API with some changes to support native AOT with .NET 8

## Codebreaker diagrams

[Docker containers](containerdiagram.md)
