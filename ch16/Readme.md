# Chapter 16 - Running applications on-premises and in the cloud

## Technical Requirements

See [Installation](../installation.md) on how to install Visual Studio, Docker Desktop, and .NET Aspire.

[Aspir8](https://github.com/prom3theu5/aspirational-manifests) is used in this chapter.

Install as a global tool:

```bash
dotnet tool install -g aspirate --prerelease
```

The code for this chapter can be found in the following GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure.

Chapter 16 contains code from the previous chapter.

## Deploying to Kubernetes with Aspir8

1. generate the manifest for the AppHost with the OnPremises launch profile:

```bash
dotnet run --launch-profile OnPremises -- --publisher manifest --output-path onpremises-manifest.json
```

2. Initialize Aspirate

```bash
aspirate init
```

Enter the URL of the ACR

3. Generate Kubernetes manifests 

```bash
aspirate generate --aspire-manifest aspire-manifest.json --output-path ./kustomize-output -skip-build --namespace codebreakerns
```

4. Bulid and push Docker images to the ACR

```bash
az acr login –name <yourregistry>
aspirate build --aspire-manifest aspire-manifest.json --container-image-tag 3.8 --container-image-tag latest --container-registry <yourregistry>.azurecr.io
```

5. Deploy to Kubernetes

```bash
aspirate apply --input-path kustomize-output
kubectl get deployments --namespace codebreakerns
kubectl get services --namespace codebreakerns
```

## Deploy the application to Microsoft Azure

[See Deploy the application to Azure using azd](../Deploy2Azure.md)
