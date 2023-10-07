# Bicep

## Resource Group

You need to create a resource group.
This can e.g. be done via the Azure portal or via the Azure CLI, as shown in the following command:

```
az group create --name <resource-group-name> --location <location>
e.g.  az group create --name my-codebreaker-sample-rg --location westeurope
```

## Key Vault

The key vault will be used to store the administrator password for the SQL server, which is necessary when creating a SQL server in Azure.  
If you would rather not use the key vault in this deployment, you can simply provide the administrator password for the SQL server as a parameter in the _codebreaker.bicepparam_.
Keep in mind, that you must not commit the password. Otherwise, it will be exposed in your source control!

Create the key vault, running the _keyvault_ Bicep script using the following command:
```
az deployment group create -g <resource-group-name> -f keyvault.bicep
```
Parameters:
- name: string
- location: string
- sqlServerAdminPassword: string
- adminPrincipalIds: string[]  
  _You can get your principalId by running `az ad signed-in-user show --query id`_
- sqlServerAdminPasswordSecretName: string (optional) (default: sqlAdminPassword)
- tenantId: string (optional) (default: The tenant id of your current subscription)

You can specify the parameters using the `-p` flag, otherwise you will be prompted to enter the required parameters.


## Codebreaker
The necessary resources for Codebreaker can be deployed using the _codebreaker.bicep_ script.
Before running that script, have a look at the parameter file _codebreaker.bicepparam_ and fill out the empty parameters.
It specifies the minimum necessary parameters for _codebreaker.bicep_.

After that, you can deploy the resources by running the following command:
```
az deployment group create -g <resource-group-name> -f codebreaker.bicep -p codebreaker.bicepparam
```
