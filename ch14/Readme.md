# Chapter 14 - gRPC for binary communication

## Technical Requirements

With this chapter, similar to the previous chapter, you need an Azure subscription, .NET Aspire, the Azure CLI, the Azure Developer CLI.

See [Installation](../installation.md) on how to install Visual Studio, Docker Desktop...

The code for this chapter can be found in the following GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure.

The important projects part of this chapters repository folder are

- Codebreaker.AppHost - the .NET Aspire host project. The app model is enhanced by an additional project running a SignalR hub and using Azure SignalR Services.
- Codebreaker.Live – the project we created in the previous chapter is changed to offer a gRPC service instead of a REST service.
- Codebreaker.GameAPIs – this project is updated to include a gRPC client to invoke the live service. In addition to the REST service used by many different clients, as an alternative a gRPC service is added which is invoked by the bot service.
- Codebreaker.Bot - the bot service is updated to use a gRPC client instead of REST to invoke the game APIs service.
- LiveTestClient – you use the live test client from the previous chapter to verify if the SignalR service.

Working through the code with this chapter, you can start using the service, bot, and live projects from the previous chapter.

## Deploy the application to Microsoft Azure

[See Deploy the application to Azure using azd](../Deploy2Azure.md)

## gRPC Service Discovery

With .NET Aspire Preview 7, https+http://gameapis cannot be used (see Bot, and GameApis - ApplicationServices.cs). The sample code uses `https://gameapis` and `https://live`.

See https://github.com/PacktPublishing/Pragmatic-Microservices-with-CSharp-and-Azure/issues/89

An alternative would be to access the environment variable, and use the binding accordingly.

Expecting updates with .NET Aspire 8.1
