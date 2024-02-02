# Deploy to Microsoft Azure using the Azure Developer CLI

> This information can be used with the samples from multiple chapters. Tested with chapter 12. 

> This will change with .NET Aspire Preview 4. Instructions created with .NET Aspire Preview 2.

## Prepare the application

1. Create an Azure Cosmos DB account (you can use the Azure portal or bicep scripts)
2. Create an Azure App Insights resource
3. Configure the Azure Cosmos DB connection and Azure App Insights with user secrets of the App Host application

```json
{
  "SqlPassword": "Password123!Password123",
  "CosmosConnectionString": <enter your connection string to Azure Cosmos DB>,
  "ApplicationInsightsConnectionString": <etner your intstrumentation connection string>
}
```

4. Change to the directory of the AppHost project `cd Codebreaker.AppHost`

5. Run ```azd init```

    1. Select `Use code in the current directory`
    2. Confirm using *Azure Container Apps*
    3. Select *bot* and *gameapis* to be reachable from the Internet
    4. Enter an environment name, e.g. *codebreaker-dev*

These files are generated:
* azure.yml - 
* next-steps.md - documentation what can be done next
* .gitignore - ignoring the .azure folder because it could contain secrets
* .azure - folder with the generated files, configuration of environment variables, and exposed services

## Deploy the appliation to Azure

1. Change to the directory of the AppHost project `cd Codebreaker.AppHost`
2. Run ```az up```
    1. Select the subscription
    2. Select a location near you
    3. Wait some minutes until the resources are created

These resources are now available:

* Resource group
* Container Registry
* Log Analytics workspace
* Container Apps Environment
* Container App *gameapis*
* Container App *bot
* Container App *redis* (starting with chapter 12)
