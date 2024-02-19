# Deploy to Microsoft Azure using the Azure Developer CLI

> This information can be used with the samples from multiple chapters.

> This will change with .NET Aspire Preview 4. Instructions created with .NET Aspire Preview 3 and Azure Developer CLI (azd) 1.6.1.

## Prepare the application

1. Create an Azure App Insights resource (starting with chapter 11, not needed before that)
2. With the AppHost, configure user secrets with this configuration:

```json
{
  "ApplicationInsightsConnectionString": <etner your intstrumentation connection string>
}
```

3. Change to the directory of the solution

4. Run ```azd init```

    1. Select `Use code in the current directory`
    2. Confirm using *Azure Container Apps*
    3. Select *bot*, *gameapis*, and *blazor* (in the chapters available) to be reachable from the Internet
    4. Enter an environment name, e.g. *codebreaker-dev*

These files are generated:
* azure.yml - 
* next-steps.md - documentation what can be done next
* .gitignore - ignoring the .azure folder because it could contain secrets
* .azure - folder with the generated files, configuration of environment variables, and exposed services

## Deploy the appliation to Azure

1. Change to the directory of the solution
2. Run ```az up```
    1. Select the subscription
    2. Select a location near you
    3. Wait some minutes until the resources are created

> Issue with azd 1.6.1: After creating the resources, you might need to change the Ingress configuration to have the services previously specified  

These resources are now available:

* Resource group
* Container Registry
* Log Analytics workspace
* Container Apps Environment
* Container App *gameapis*
* Container App *bot*
* Container App *blazor* (starting with chapter 12)
* Container App *redis* (starting with chapter 12)

## Create a container with the Azure Cosmos DB database

1. Create a container named `GamesV3`
2. Specify the partition key name `/PartitionKey`

## Delete the Azure resources

After you don't need them anymore, delete the Azure resources using

`azd down`
