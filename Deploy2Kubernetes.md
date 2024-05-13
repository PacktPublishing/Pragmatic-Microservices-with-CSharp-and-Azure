# Deploy to Kubernetes using Aspir8

> This information is used with Chapter 16

## Installation of the aspirate Tool

Install as a global tool:

```bash
dotnet tool install -g aspirate --prerelease
```

## Create ACR

See chapter 16

## Create AKS

See chapter 16

## Deploy to AKS

### Create a .NET Aspire manifest

Generate the manifest for the AppHost with the OnPremises launch profile (Use the code from chapter 16):

```bash
cd Codebreaker.AppHost
dotnet run --launch-profile OnPremises -- --publisher manifest --output-path onpremises-manifest.json
```

2. Initialize Aspirate

```bash
aspirate init
```

* fall-back value for the container builder? Enter **n** (use Docker Desktop)
* fall-back value for the container registry? **y**
* Enter the container registry: **enter the link to your ACR**, e.g. acrcodebreaker.azurecr.io
* Repository prefix? **n**
* Use a custom directory for * Templates* when generating kustomize manifests? **n**

This creates the file `aspirate.json` with the configurations.

3. Generate Kubernetes manifests 

```bash
aspirate generate --aspire-manifest onpremises-manifest.json --output-path ./kustomize-output --skip-build --namespace codebreakerns
```

Select all components.
Would you like to deploy the aspire dashboard and connect the OLTP endpoint? Enter **y**
For storing the secrets, enter a password.
Generate top level kustomize manifest to run against your kubernetes cluster? Enter **y**

4. Bulid and push Docker images to the ACR

```bash
az acr login –-name <yourregistry>
aspirate build --aspire-manifest onpremises-manifest.json --container-image-tag 3.8 --container-image-tag latest --container-registry <yourregistry>.azurecr.io
```

Use all previous state values where possible? Enter **y**

5. Deploy to Kubernetes

```bash
aspirate apply --input-path kustomize-output
kubectl get deployments --namespace codebreakerns
kubectl get services --namespace codebreakerns
```

Use all previous state values where possible? Enter **y**
Deploy the generated manifests to a kubernetes cluster defined in your kubeconfig file? Enter **y**
