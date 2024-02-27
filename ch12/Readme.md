# Chapter 12 - Scaling

## Technical Requirements

With this chapter, like the previous chapters, you need an Azure subscription, the Azure Developer CLI (winget install Microsoft.Azd) and Docker desktop.
The code for this chapter can be found in this GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure
In the ch12 folder, you’ll see these projects with the final result of this chapter. To add the functionality from this chapter, you can start with the source code from the previous chapter.

The important projects part of this chapters repository folder are

*	Codebreaker.AppHost - the .NET Aspire host project. This project is enhanced by adding a Redis resource for caching.
*	Codebreaker.ServiceDefaults – here we use common health check configuration for all the services.
*	Codebreaker.GameAPIs – with this project we implement caching games to reduce database access and add a custom health check.

> Running the local Azure Cosmos DB emulator, uncomment the corresponding code within ApplicationServices.cs! With preview 4, this will not be necessary - the Azure Cosmos Docker container emulator is already configured. With Aspire Preview 4, the code will be updated, and the emulator locally started within a Docker container. With Preview 3 there's also the issue that `azd up publishes debug build. Fixed with https://github.com/Azure/azure-dev/pull/3443

## Deploy the application to Azure

[See Deploy the application to Azure using azd](../Deploy2Azure.md)

## Azure Load Testing test requests

To create the URL based Azure Load Test, you can use this information.

### Create game

Request name: Create game
URL: <link to your games API ACA>/games
HTTP method: POST
Headers:
- content-type application/json
- accept application/json
Body:

```json
{
  "gameType": "Game6x4",
  "playerName": "Test"
}
```

Response variable: 
Type: JSONPath
Name: gameId
Expression: $.id

### Set move 1

Request name: Set move 1
URL: <link to your games API ACA>/games/${gameId}
HTTP method: PATCH
Headers:
- content-type application/json
- accept application/json
Body:

```json
{
  "id": "${gameId}",
  "gameType": "Game6x4",
  "playerName": "test",
  "moveNumber": 1,
  "end": false,
  "guessPegs": [
    "Red",
	  "Red",
	  "Red",
	  "Red"
  ]
}
```

### Get game

Request name: Get game
URL: <link to your games API ACA>/games/${gameId}
HTTP method: GET
Headers:
- accept application/json

### End game

Request name: End game
URL: <link to your games API ACA>/games/${gameId}
HTTP method: PATCH
Headers:
- content-type application/json
- accept application/json
Body:

```json
{
  "id": "${gameId}",
  "gameType": "Game6x4",
  "playerName": "test",
  "moveNumber": 2,
  "end": true
}
```

### Delete game

Request name: Delete game
URL: <link to your games API ACA>/games/${gameId}
HTTP method: DELETE
