metadata description = 'Creates an Azure Key Vault'

targetScope = 'resourceGroup'

@description('Specifies the name of the Key Vault.')
param vaultName string = 'kv${uniqueString((resourceGroup().id))}'

@description('Specifies the Azure location where the app configuration store should be created.')
param location string = resourceGroup().location

@description('Specifies the SKU of the app configuration store.')
param keyVaultSku object = {
  name: 'standard'
  family: 'A'
}

@description('The managed identity to get access to the key vault secrets.')
param managedIdentityName string

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: vaultName
  location: location
  properties: {
    sku: keyVaultSku
    enabledForTemplateDeployment:true
    enableRbacAuthorization:true
    tenantId: tenant().tenantId
  }
}

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: managedIdentityName
}


var roleAssignmentId = guid(substring(vaultName, 0, 10), managedIdentity.id, 'KeyVaultSecretsUser')

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: roleAssignmentId
  scope: keyVault
  properties: {
    principalId: managedIdentity.properties.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // Key Vaults secret user role ID https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
  }
}
