# Chapter 10 - All about testing the solution

## Technical Requirements

With this chapter, similar to the previous chapter, you need an Azure subscription. Docker Desktop, and .NET Aspire.

The code for this chapter can be found in this GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure
In the ch10 folder, you’ll see these projects with the final result of this chapter. 

These projects are unchanged from previous chapters:

* Codebreaker.AppHost - the .NET Aspire host project
* Codebreaker.ServiceDefaults - common service configuration
* Codebreaker.Bot - the bot service to run games

These projects are unchanged from previous chapters, but of special interest for the tests:

* Codebreaker.Analyzers - this is the project that contains analyzers to verify game moves and returns the results
* Codebraker.GameApis - the games API service project

These projects are new:

* Codebreaker.Analyzers.Tests - unit tests for the analyzer library
* Codebreaker.Bot.Tests - unit tests for the bot service library
* Codebreaker.GameAPIs.Tests - unit tests for the games services project
* Codebreaker.GameAPIs.IntegrationTests - in-memory integration test
* Codebreaker.GameAPIs.Playwright - Playwright used for end-to-end testing

Working through the code with this chapter, you can start using the Start folder which contains the same projects without the test projects.

## Deploy the application to Azure

[See Deploy the application to Azure using azd](../Deploy2Azure.md)
