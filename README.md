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

No. The book covers .NET Aspire by using Microsoft Azure services, and as an alternative option, services are built by hosting the applicaiton in an on-premises environment.

.NET Aspire allows creating a manifest of the applications and services defined in the app-model. This manifest is independent of any platform specificas - unless Azure services are used with the app model. In case Azure services are used by the app-model it's easily possible to allow creating different manifests for e.g. a Kubernetes and a Microsoft Azure environment, as it's done in the book.

With the first versions of .NET Aspire, different tools are available to use this manifest, and create publish resources, or publish the solution:

- azd to create bicep scripts, and to publish the solution to Microsoft Azure
- aspirate to create resources for **docker compose** and for **Kubernetes**, and to publish the solution to **Kubernetes**

Since .NET Aspire 9.2, the **Aspire CLI** can be used, togher with **Publisher Libraries** as defined by the app model:

```csharp
builder.AddDockerComposePublisher();
builder.AddKubernetesPublisher();
builder.AddAzurePublisher();
```

With this, a Docker Compose file can be created, Helm charts to publish to Kubernetes, and Bicep scripts to publish to Microsoft Azure. More publishers can follow soon, such as a publisher to create Terraform - see [Support of .NET Aspire deployment using terraform](https://github.com/dotnet/aspire/issues/6559)

### Does .NET Aspire replace Kubernetes?

No, we still need a hosting environment where the solution can run. This can be orchestration with *Kubernetes*, using *Azure Container Apps* where Kubernetes is used behind the scenes, and other hosting environments. 

On the local developer system, when just .NET projects are referenced, Docker is not required. In case .NET integrations such as a Docker container for SQL Server or Redis are used, .NET Aspire works with **Docker Desktop** or **Podman** installed locally.

[WebAssembly with .NET Aspire](https://www.infoq.com/articles/webassembly-containers-dotnet-aspire) can be another option.

### Does .NET Aspire replace Docker Compose?

No. But .NET Aspire makes it easy to create *Docker Compose* files, using the *Aspire CLI* with this app model definition:

```csharp
builder.AddDockerComposePublisher();
```

### Is .NET Aspire only for .NET projects?

No. In one way it's possible to add Docker images to the app model, but it's also possible to add projects developed using other programming languages to the app model.

Check the [.NET Aspire Community Toolkit](https://github.com/CommunityToolkit/Aspire) for adding **Rust**, **Python**, **Go**, **Java**, **NodeJS**, and more!

### Does .NET Aspire replace Prometheus, Grafana, and Jaeger

No. .NET Aspire offers a dashboard to see logging, metrics, and distributed tracing which originally was only planned to be used during development. During development this makes it easy to see memory leaks, where time is spent, and more - which helps a lot while working on some fixes and feature updates. Because this dashboard is so great, many requested to use this in production which is now possible. The dashboard is now pre-installed with an *Azure Container Apps* environment, and can be used as standalone executable or a Docker container. 

Logging data is not persisted with the .NET Aspire dashboard, thus other services such as Prometheus, Jaeger, and Grafana are still necessary.

### Can .NET Aspire be used with AWS?

Yes! Check this blog article [Integrating AWS with .NET Aspire](https://aws.amazon.com/blogs/developer/integrating-aws-with-net-aspire/), and this GitHub Repo [Integrations on .NET Aspire for AWS GitHub Repo](https://github.com/aws/integrations-on-dotnet-aspire-for-aws)

The NuGet package [https://www.nuget.org/packages/Aspire.Hosting.AWS](https://www.nuget.org/packages/Aspire.Hosting.AWS) offers provisioning resources with *AWS CloudFormation*, and with *AWS CDK*. It offers integrations for *AWS Lambda*, *Amazon DynamoDB*, and more!

### Does .NET Aspire useful just for DevOps and not Developers?

As now there's such a focus on publishing with .NET Aspire to different environments, I get questions such as: "Is it useful for developers, or just DevOps?" Check my blog article [Why should I use .NET Aspire?](https://csharp.christiannagel.com/2025/05/08/why-dotnet-aspire/). Of course, .NET Aspire is useful for developers. It makes it easy to add services using .NET Aspire integrations, and finding issues early before issues are returned from the CI/CD pipelines.
