# Chapter 2

## Technical requirements

The code for this chapter can be found in the following GitHub repository:
https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure
The source code folder ch02 contains the code samples for this chapter. You’ll find the code for four projects:

* Codebreaker.GamesAPIs – this is the Web API project.
* Codebreaker.GamesAPIs.Models – a library for the data models
* Codebreaker.GameAPIs.Algorithms – a library containing algorithms for the game
* Codebreaker.GamesAPIs.Algorithms.Tests – unit tests for the algorithms

You don’t implement the algorithms of the game in this chapter. The algorithms project is just for reference purposes, but you can simple use a NuGet package for the algorithms that has been made available for you to build the service.

## Changes after first draft

These changes will be reflected in the next version of the book chapter:

* new JsonStringEnumConverter<T> instead of JsonStringEnumMemberConverter now used as attibute instead of method invocations
* UpdateGameRequest and UpdateGameResponse instead of SetMoveRequest and SetMoveResponse
* GamesQuery for querying games
* The analyzer package has the new name CNinnovation.Codebreaker.Analyzers 
