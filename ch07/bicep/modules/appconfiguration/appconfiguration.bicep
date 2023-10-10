metadata description = 'Creates an Azure App Configuration'

targetScope = 'resourceGroup'

@description('Specifies the name of the App Configuration store.')
param configStoreName string

@description('Specifies the Azure location where the app configuration store should be created.')
param location string = resourceGroup().location

@description('Specifies the SKU of the app configuration store.')
param skuName string = 'Standard'

@description('Specifies the name of the principal id (GUID) that gets permission to access the app configuration.')
param principalId string

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
    value: 'value1'
  }
}

resource botConfigValue 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-03-01' = {
  parent: appConfig
  name: 'BotService'
  properties: {
    value: 'value2'
  }
}

var roleAssignmentId = guid(substring(configStoreName, 0, 10), principalId, 'AppConfigDataReader')

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: roleAssignmentId
  scope: appConfig
  properties: {
    principalId: principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '516239f1-63e1-4d78-a4de-a74fb236a071') // App Configuration Reader role ID https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
  }
}
