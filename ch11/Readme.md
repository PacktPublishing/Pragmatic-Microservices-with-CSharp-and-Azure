# Chapter 11 - Logging and monitoring

## Technical Requirements

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

## Running the solution

To run the solution, running the application with Prometheus and Grafana, switch to the *Prometheus* environment (see `launchsettings.json` with the AppHost project)
