targetScope = 'resourceGroup'

@description('')
param location string = resourceGroup().location

@description('')
param sku string = 'Standard'

@description('')
param principalId string

@description('')
param principalType string


resource eventHubsNamespace_1iqfOzI6f 'Microsoft.EventHub/namespaces@2021-11-01' = {
  name: toLower(take('codebreakerevents${uniqueString(resourceGroup().id)}', 24))
  location: location
  tags: {
    'aspire-resource-name': 'codebreakerevents'
  }
  sku: {
    name: sku
  }
  properties: {
  }
}

resource roleAssignment_s3SgpSXPk 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: eventHubsNamespace_1iqfOzI6f
  name: guid(eventHubsNamespace_1iqfOzI6f.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'f526a384-b230-433a-b45c-95f59c4a2dec'))
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'f526a384-b230-433a-b45c-95f59c4a2dec')
    principalId: principalId
    principalType: principalType
  }
}

resource eventHub_WaongjwAy 'Microsoft.EventHub/namespaces/eventhubs@2021-11-01' = {
  parent: eventHubsNamespace_1iqfOzI6f
  name: 'games'
  location: location
  properties: {
  }
}

output eventHubsEndpoint string = eventHubsNamespace_1iqfOzI6f.properties.serviceBusEndpoint
