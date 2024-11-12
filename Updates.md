# Aspire updates

What's changed with the new versions of .NET Aspire?

## Chapter 1, Introdution to .NET Aspire and Microservices

### Page 4, Starting with .NET Aspire:

The *Aspire workload* is no longer required with Aspire 9. Instead, the Aspire SDK is specified with the AppHost project file:

```xml
 <Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />
```

### Page 7, The app model with the generated code:

```csharp
builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);
```

The WaitFor method is new with .NET Aspire 9 which allows waiting with the start of the webfrontend until the apiService has been started.

### Page 13, .NET Aspire integrations

*.NET Aspire components* have been renamed to *.NET Aspire integrations*
