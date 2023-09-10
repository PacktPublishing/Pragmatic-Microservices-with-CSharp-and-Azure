# Bicep

## Resource Group

You need to create a resource group.
This can e.g. be done via the Azure portal or via the Azure CLI, as shown in the following command:

```
az group create --name <resource-group-name> --location <location>
e.g.  az group create --name rg-my-codebreaker-sample --location westeurope
```

## Codebreaker

The necessary resources for Codebreaker can be deployed using the _codebreaker.bicep_ script.
Before running that script, have a look at the parameter file _codebreaker.bicepparam_ and fill out the empty parameters.
It specifies the minimum necessary parameters for _codebreaker.bicep_.

After that, you can deploy the resources by running the following command:
```
az deployment group create -g <resource-group-name> -f codebreaker.bicep -p codebreaker.bicepparam
```
