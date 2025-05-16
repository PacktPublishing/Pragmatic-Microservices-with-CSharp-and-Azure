# Pragmatic Microservices with CSharp and Azure

[Pragmatic Microservices with CSharp and Azure](https://www.packtpub.com/en-us/product/pragmatic-microservices-with-c-and-azure-9781835088296)

Welcome to this repository with the source code for the book!

## .NET Aspire Updates

[See updates](Updates.md) with all the chapters based on new .NET Aspire features, as well as other .NET updates.

Also check these articles on updates and new features:

- [Key Features of .NET Aspire 9.2: Enhance Your Microservices Part 1](https://csharp.christiannagel.com/2025/04/12/pragmaticmicroervices-part1-aspire92/)

## Installation requirements

[Tools needed to build and run the application](installation.md)

> Check this information about [Updates with .NET Aspire 9](Updates.md) 

## Questions?

In case you've issues building or running the Codebreaker application, please use [GitHub Discussions](https://github.com/PacktPublishing/Pragmatic-Microservices-with-CSharp-and-Azure/discussions)!

## Chapters

* Chapter 1 - Introduction to .NET Aspire and Microservices
* Chapter 2 - Minimal APIs – Creating REST Services
* Chapter 3 - Writing data to relational and NoSQL databases
* Chapter 4 - Creating Libraries for Client Applications
* Chapter 5 - Containerization of Microservices
* Chapter 6 - Microsoft Azure for Hosting of Applications
* Chapter 7 - Flexible Configuration
* Chapter 8 - CI/CD with GitHub Actions
* Chapter 9 - Authentication and Authorization with clients and services
* Chapter 10 - All about testing the solution
* Chapter 11 - Logging and monitoring
* Chapter 12 - Scaling Services
* Chapter 13 - Real-time messaging with SignalR
* Chapter 14 - gRPC for binary communication
* Chapter 15 - Asynchronous communication with messages and events
* Chapter 16 - Running the application on-premises and in the cloud

## Deploying and running the application

[Run the application with Azure resources from the local development environment](RunDevEnvironment.md)

[Deploying the application to Azure](Deploy2Azure.md)

[Deploying the application to Kubernetes](Deploy2Kubernetes.md)

## More information

To see as the application develops further, and client applications using Blazor, WinUI, WPF, .NET MAUI, uno Platform, AvaloniaUI check the [CodebreakerApp Org](https://github.com/CodebreakerApp).

## .NET Aspire FAQ

The book covers creating services using .NET Aspire, starting by creating REST APIs, using databases, and with every chapter more and more features are added. Using .NET Aspire, there are some confusions, as .NET Aspire covers quite a lot. Questions I get in workshops are answered here:

### Does .NET Aspire need Microsoft Azure?

No. The book covers .NET Aspire by using Microsoft Azure services, and as an alternative option, services are built by hosting the application in an on-premises environment.

.NET Aspire allows creating a manifest of the applications and services defined in the app-model. This manifest is independent of any platform specifics—unless Azure services are used with the app model. If Azure services are used, it's easy to create different manifests for, e.g., a Kubernetes and a Microsoft Azure environment, as demonstrated in the book.

With the first versions of .NET Aspire, different tools are available to use this manifest and create publish resources, or publish the solution:

- **azd** to create Bicep scripts and publish the solution to Microsoft Azure
- **aspirate** to create resources for **Docker Compose** and **Kubernetes**, and to publish the solution to **Kubernetes**

Since .NET Aspire 9.2, the **Aspire CLI** can be used, together with **Publisher Libraries** as defined by the app model:

```csharp
builder.AddDockerComposePublisher();
builder.AddKubernetesPublisher();
builder.AddAzurePublisher();
```

With this, a Docker Compose file can be created, Helm charts to publish to Kubernetes, and Bicep scripts to publish to Microsoft Azure. More publishers can follow soon, such as a publisher to create Terraform—see [Support of .NET Aspire deployment using terraform](https://github.com/dotnet/aspire/issues/6559).

> **Azure Best Practice:** When deploying to Azure, use managed identities and Key Vault for secrets management. Always review [Azure Well-Architected Framework](https://learn.microsoft.com/azure/architecture/framework/) for guidance.

### Does .NET Aspire replace Kubernetes?

No, you still need a hosting environment where the solution can run. This can be orchestration with *Kubernetes*, using *Azure Container Apps* (where Kubernetes is used behind the scenes), or other hosting environments.

On the local developer system, when just .NET projects are referenced, Docker is not required. If .NET integrations such as a Docker container for SQL Server or Redis are used, .NET Aspire works with **Docker Desktop** or **Podman** installed locally.

[WebAssembly with .NET Aspire](https://www.infoq.com/articles/webassembly-containers-dotnet-aspire) can be another option.

### Does .NET Aspire replace Docker Compose?

No. But .NET Aspire makes it easy to create *Docker Compose* files, using the *Aspire CLI* with this app model definition:

```csharp
builder.AddDockerComposePublisher();
```

### Is .NET Aspire only for .NET projects?

No. You can add Docker images to the app model, and also add projects developed using other programming languages.

Check the [.NET Aspire Community Toolkit](https://github.com/CommunityToolkit/Aspire) for adding **Rust**, **Python**, **Go**, **Java**, **NodeJS**, and more!

### Does .NET Aspire replace Prometheus, Grafana, and Jaeger?

No. .NET Aspire offers a dashboard to see logging, metrics, and distributed tracing, which was originally only planned for development. This dashboard is now pre-installed with an *Azure Container Apps* environment and can be used as a standalone executable or a Docker container.

Logging data is not persisted with the .NET Aspire dashboard, so other services such as Prometheus, Jaeger, and Grafana are still necessary for production monitoring.

> **Azure Best Practice:** For production, integrate with Azure Monitor, Application Insights, and Azure Log Analytics for end-to-end observability.

### Can I host .NET Aspire applications with Internet Information Server (IIS)?

Yes, you can host .NET Aspire applications with IIS. You might not yet use Docker or containerized services like Redis yet. In this scenario, you might have a .NET Web API or front-end application that can be deployed using traditional IIS mechanisms.

Just don't try to use this with old .NET Framework ASP.NET applications. Migrate to the new .NET before!

.NET Aspire projects added to the app host are standard .NET applications and can be published and deployed to IIS as you would with any ASP.NET Core application. Service discovery in .NET Aspire (e.g., a web app referencing an API project) falls back to configuration, so you can configure endpoints and connection strings in your IIS environment.

You can also configure existing database connections using the app model. 

The main advantages of .NET Aspire in this setup are:

- Using the Aspire dashboard during development for enhanced observability.
- Simplified integration with database connection strings and other resources.
- Gradual adoption of cloud-native patterns while still supporting traditional hosting.

Using .NET Aspire in this environment can be a first step when thinking about a transition to container-based hosting for greater scalability and flexibility in the future.

> **Note:** Some Aspire features (like dynamic service discovery or container orchestration) are not fully leveraged in a pure IIS environment. However, Aspire can still streamline configuration and local development.

### Can .NET Aspire be used with AWS?

Yes! Check this blog article [Integrating AWS with .NET Aspire](https://aws.amazon.com/blogs/developer/integrating-aws-with-net-aspire/), and this GitHub Repo [Integrations on .NET Aspire for AWS GitHub Repo](https://github.com/aws/integrations-on-dotnet-aspire-for-aws).

The NuGet package [Aspire.Hosting.AWS](https://www.nuget.org/packages/Aspire.Hosting.AWS) offers provisioning resources with *AWS CloudFormation* and *AWS CDK*. It offers integrations for *AWS Lambda*, *Amazon DynamoDB*, and more!

### Is .NET Aspire useful just for DevOps and not Developers?

No, .NET Aspire is valuable for both developers and DevOps. It simplifies adding services, integrations, and infrastructure as code, making it easier to find and fix issues early in development before they reach CI/CD pipelines. See [Why should I use .NET Aspire?](https://csharp.christiannagel.com/2025/05/08/why-dotnet-aspire/).

### How do I secure secrets and connection strings in Azure deployments?

Use [Azure Key Vault](https://learn.microsoft.com/azure/key-vault/general/basic-concepts) to store secrets, certificates, and connection strings. Reference secrets in your application configuration using managed identities for secure, passwordless access.

### What are the recommended practices for scaling microservices on Azure?

- Use Azure Kubernetes Service (AKS) or Azure Container Apps for orchestration and scaling.
- Implement autoscaling based on CPU, memory, or custom metrics.
- Use Azure Service Bus or Azure Event Grid for decoupled, scalable messaging.
- Monitor with Azure Monitor and set up alerts for proactive scaling.

### Can I use GitHub Actions for CI/CD with .NET Aspire and Azure?

Yes! GitHub Actions can build, test, and deploy your .NET Aspire applications to Azure. Use the [Azure/login](https://github.com/Azure/login) and [Azure/webapps-deploy](https://github.com/Azure/webapps-deploy) actions for authentication and deployment. See [CI/CD with GitHub Actions](https://learn.microsoft.com/azure/developer/github/).

### Where can I find more Azure best practices for microservices?

- [Azure Microservices Architecture Guide](https://learn.microsoft.com/azure/architecture/microservices/)
- [Azure Well-Architected Framework](https://learn.microsoft.com/azure/architecture/framework/)
- [Cloud Adoption Framework](https://learn.microsoft.com/azure/cloud-adoption-framework/)
---
