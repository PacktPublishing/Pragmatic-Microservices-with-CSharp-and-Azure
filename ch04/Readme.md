# Chapter 4

## Technical requirements

The code for this chapter can be found in the following GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure
The source code folder ch04 contains the code samples for this chapter. 

The source code folder ch04 contains the code samples for this chapter. 
The service didn’t change from the previous chapter and is stored in the server folder. 
Contrary to the previous chapter, the data projects are not included, instead NuGet packages are used.
You can use the file `Chapter04.server.sln` to open and run the solution. You need to start the service when running the client application. Based on your preference, you need to configure SQL Server or Azure Cosmos DB as discussed in the previous chapter. You can also use the in-memory repository instead, then you don’t need to have a database running. Change the configuration with the appsettings.json file based on your needs.
Configuring the `DataStore` setting to `InMemory`, you don't need to start Docker.

The new code is in the client folder. Here you find these projects:

* Codebreaker.GameApis.Client – this is the new library which includes custom models and the GamesClient class which sends HTTP requests to the service.
* Codebreaker.Client.Console – a new console application which references the client library and can be used to play the game.
* Codebreaker.GamesApis.Kiota – this is a client library which can be used as an alternative to Codebreaker.GameApis.Client with generated code.
* Codebreaker.Kiota.Console – a console application which uses the Kiota client library.
