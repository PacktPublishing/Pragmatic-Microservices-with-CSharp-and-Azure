# Chapter 15 - Asynchronous Communication with messages and events

## Technical Requirements

With this chapter, similar to the previous chapter, you need an Azure subscription, .NET Aspire, the Azure CLI, the Azure Developer CLI.

See [Installation](../installation.md) on how to install Visual Studio, Docker Desktop...

The code for this chapter can be found in the following GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure.

The important projects part of this chapters repository folder are

-	Codebreaker.AppHost - the .NET Aspire host project. The app model is enhanced by adding Azure Storage, Azure Event Hub, and Kafka services.
-	Codebreaker.BotQ – this is a new project with nearly the same code as Codebreaker.Bot – but instead of using a REST API to trigger the game plays, a message queue is used.
-	Codebreaker.GameAPIs – this project is updated to forward completed games not directly to the live service, but publishing events to Azure Event Hub, or with a different start of the application to Kafka.
-	Codebreaker.Live – this project is changed to subscribe events from Azure Event Hub using async streaming. The SignalR implementation is changed as well to use async streaming.
-	Codebreaker.Ranking – this is a new project receiving events from Azure Event Hub or Kafka, writes this information to an Azure Cosmos DB database, and offers a REST service to retrieve the rank of the day. With the Event Hub we use a different way to receive events than with the live service.

Working through the code with this chapter, you can start using the service, bot, and live projects from the previous chapter.

## Ranking Azure Cosmos database

The EF Core context method `GetGameSummariesByDayAsync` needs a composite index with the query.

Currently it cannot be configured using EF Core: https://github.com/dotnet/efcore/issues/17303

This index needs to be specified with the Cosmos database (add this using the Azure portal, or using the emulator):

```json
"compositeIndexes": [
  [
    {
      "path": "/NumberMoves",
      "order": "ascending"
    },
    {
      "path": "/Duration",
      "order": "ascending"
    }
  ]
]
```

## Deploy the application to Microsoft Azure

[See Deploy the application to Azure using azd](../Deploy2Azure.md)
