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

## Playwright Testing

> Playwright Testing is currently in preview, Playwright Testing with .NET is experiemental.

### Preparation

TODO: add instructions to deploy the games API service after azd #49

1. Create a **Playwright Testing** resource with the [Azure portal](https://portal.azure.com)
2. Signin to the [Playwright portal](https://playwright.microsoft.com/)
3. Select your Azure Subscription where you created the Playwright resource

### Configure and start the tests

Change to the directory of the project *Codebreaker.GamesAPI.Playwright*.

1. Change appsettings.json file to reference your Games API service running with Azure Container Apps.
2. Install required browsers:

```powershell
pwsh bin/Debug/net8.0/playwright.ps1 install
```

3. Set access token generated in the Playwright portal as environment variable for your project: 

 ```powershell
    $env:PLAYWRIGHT_SERVICE_ACCESS_TOKEN= # Paste Access Token value from previous step
 ```
    
4. In the [Playwright portal](https://aka.ms/mpt/portal), copy the command under **Add region endpoint in your set up** and set the following environment variable:

```powershell
$env:PLAYWRIGHT_SERVICE_URL= # Paste region endpoint URL
```

5. Start the tests

```powershell
dotnet test -- NUnit.NumberOfTestWorkers=50
```

Check that the tests run successfully, monitor the metrics data of your Azure Container App.

### More information

[Get started with Playwright](https://github.com/microsoft/playwright-testing-service/blob/main/README.md#get-started)

See instructions from the Playwright team [Scale Playwright .NET tests with Microsoft Playwright testing](https://github.com/microsoft/playwright-testing-service/tree/main/samples/.NET/NUnit)
