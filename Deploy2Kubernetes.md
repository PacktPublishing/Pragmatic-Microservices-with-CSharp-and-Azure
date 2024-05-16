# Deploy to Kubernetes using Aspir8

> This information is used with Chapter 16

## Installation of the aspirate Tool

Install as a global tool:

```bash
dotnet tool install -g aspirate --prerelease
```

IF you've already installed it, a new version might be available. Checkt the current version and the version available, and update if a new version is available:

```bash
aspirate --version
dotnet tool search aspirate --prerelease
dotnet tool update -g aspirate --prerelease
```

## Create ACR

See chapter 16

After creating the Kubernetes cluster, don't forget to configure the default Kubernetes cluster in the kubeconfig file:

```bash
az aks get-credentials --resource-group <your resource group> --name <your aks name>
```

## Create AKS

See chapter 16

## Deploy to AKS

Optionally, create a .NET Aspire manifest. This manifest can be used with Aspir8. Insted, we'll use the launch profile starting the application. This file can be referenced from the aspirate tool:

```bash
cd Codebreaker.AppHost
dotnet run --launch-profile OnPremises -- --publisher manifest --output-path onpremises-manifest.json
```

1. Initialize Aspirate

```bash
cd Codebreaker.AppHost
aspirate init --launch-profile OnPremises
```

* fall-back value for the container builder? Enter **n** (use Docker Desktop)
* fall-back value for the container registry? **y**
* Enter the container registry: **enter the link to your ACR**, e.g. acrcodebreaker.azurecr.io
* Repository prefix? **n**
* Use a custom directory for * Templates* when generating kustomize manifests? **n**

This creates the file `aspirate.json` with the configurations.

2. Generate Kubernetes manifests 

```bash
aspirate generate --launch-profile OnPremises --output-path ./kustomize-output --skip-build --namespace codebreakerns
```

Select all components.
Would you like to deploy the aspire dashboard and connect the OLTP endpoint? Enter **y**
For storing the secrets, enter a password. Remember this password.
Generate top level kustomize manifest to run against your kubernetes cluster? Enter **y**

3. Bulid and push Docker images to the ACR

```bash
az acr login –-name <yourregistry>
aspirate build --launch-profile OnPremises --container-image-tag 3.8 --container-image-tag latest --container-registry <yourregistry>.azurecr.io
```

Use all previous state values where possible? Enter **y**

4. Deploy to Kubernetes

```bash
aspirate apply --input-path kustomize-output
kubectl get deployments --namespace codebreakerns
kubectl get services --namespace codebreakerns
```

Use all previous state values where possible? Enter **y**
Deploy the generated manifests to a kubernetes cluster defined in your kubeconfig file? Enter **y**
