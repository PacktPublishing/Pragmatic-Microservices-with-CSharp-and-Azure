# Chapter 13 - Real-time messaging with SignalR

## Technical Requirements

With this chapter, similar to the previous chapter, you need an Azure subscription, .NET Aspire, the Azure CLI, the Azure Developer CLI.

See [Installation](../installation.md) on how to install Visual Studio, Docker Desktop...

The code for this chapter can be found in the following GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure.

The important projects part of this chapters repository folder are

- Codebreaker.AppHost - the .NET Aspire host project. The app model is enhanced by an additional project running a SignalR hub and using Azure SignalR Services.
- Codebreaker.Live – this is a new project hosting minimal APIs invoked by the game APIs service, and the SignalR hub.
- Codebreaker.GameAPIs – this project is enhanced forwarding completed games to the live service.
- LiveTestClient – this is a new console application registering with the SignalR hub to receive completed games.

Working through the code with this chapter, you can start using the service and bot projects from the previous chapter.

## Deploy the application to Microsoft Azure

[See Deploy the application to Azure using azd](../Deploy2Azure.md)
