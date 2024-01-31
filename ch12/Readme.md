# Chapter 12 - Scaling

## Technical Requirements

TODO: This is temporary with .NET Aspire Preview 3! Preview 4 or 5 will add Cosmos DB, Azure App Insights...

1. Create an Azure Cosmos DB account
2. Create an Azure App Insights resource

3. Configure the Azure Cosmos DB connection string with user secrets
4. Configure the Azure App Insights resource with user secrets

TODO: update this for the scaling chapter

With this chapter, like the previous chapters, you need an Azure subscription and Docker desktop. To create all the Azure resources for the solution, you can use the Azure Developer CLI – azd up creates all the resources.  
The code for this chapter can be found in this GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure
In the ch11 folder, you’ll see these projects with the result of this chapter. Multiple configurations are available, with the Debug environment Debug-Prometheus and Debug, and similar Release configurations. With the Prometheus configurations, the constant PROMETHEUS is configured. This allows us to differentiate code sections. If the PROMETHEUS constant is defined, the SQL Server database hosted in a Docker container, Prometheus, and Grafana are used. Without this constant, we use Azure Cosmos DB, and Azure Application Insights.
These are the important projects for this chapter:

* Common projects
  * Codebreaker.AppHost – the .NET Aspire host project. 
  * Codebreaker.ServiceDefaults – common service configuration. This project is enhanced with service configurations for monitoring.
* Services and clients
  * Codebreaker.GamesAPI – the service project is enhanced with logging, metrics, and distributed tracing.
  * Codebreaker.Bot – this project has monitoring information included and will be used to play games that can be monitored.
* Configuration folders
  * The grafana folder contains configuration files that are used within the Grafana Docker container.
  * The prometheus folder contains a configuration file that is used by the Prometheus Docker container.
Working through the code with this chapter, you can start using the Start folder which contains the same projects without the code which needs to be added for monitoring.

## Run the application

Deploy the application to your Azure environment:

cd Codebreaker.AppHost
azd init
azd up

Run the load test

cd Codebreaker.GameAPIs.Playwright

Change the BaseUrl configuration to reference your Azure Container App games API (appsettings.json)

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
