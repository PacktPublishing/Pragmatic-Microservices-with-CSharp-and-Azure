metadata description = 'Creates an Azure App Configuration'

targetScope = 'resourceGroup'

@description('Specifies the name of the App Configuration store.')
param configStoreName string

@description('Specifies the Azure location where the app configuration store should be created.')
param location string = resourceGroup().location

@description('Specifies the SKU of the app configuration store.')
param skuName string = 'Standard'

@description('The managed identity to get access to the key vault secrets.')
param managedIdentityName string

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2023-03-01' = {
  name: configStoreName
  location: location
  sku: {
    name: skuName
  }
}

resource gamesAPIConfigValue 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-03-01' = {
  parent: appConfig
  name: 'GamesAPI'
  properties: {
    value: 'sample value 1'
  }
}

resource botConfigValue 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-03-01' = {
  parent: appConfig
  name: 'BotService'
  properties: {
    value: 'sample value 2'
  }
}

// create a role assignment to have the principalId read access to the app configuration store
resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: managedIdentityName
}

var roleAssignmentId = guid(substring(configStoreName, 0, 10), managedIdentity.name, 'AppConfigDataReader')

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: roleAssignmentId
  scope: appConfig
  properties: {
    principalId: managedIdentity.properties.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '516239f1-63e1-4d78-a4de-a74fb236a071') // App Configuration Reader role ID https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
  }
}
