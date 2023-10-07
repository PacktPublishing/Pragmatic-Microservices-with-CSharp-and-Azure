/*
* Container Registry
*/

// Parameters
@description('The name for the container registry')
param name string

@description('The location for the container registry')
param location string = resourceGroup().location

// Resources
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2022-12-01' = {
  name: name
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

resource adminScope 'Microsoft.ContainerRegistry/registries/scopeMaps@2022-12-01' = {
  parent: containerRegistry
  name: '_repositories_admin'
  properties: {
    description: 'Can perform all read, write and delete operations on the registry'
    actions: [
      'repositories/*/metadata/read'
      'repositories/*/metadata/write'
      'repositories/*/content/read'
      'repositories/*/content/write'
      'repositories/*/content/delete'
    ]
  }
}

resource pullOnlyScope 'Microsoft.ContainerRegistry/registries/scopeMaps@2022-12-01' = {
  parent: containerRegistry
  name: '_repositories_pull'
  properties: {
    description: 'Can pull any repository of the registry'
    actions: [
      'repositories/*/content/read'
    ]
  }
}

resource pushOnlyScope 'Microsoft.ContainerRegistry/registries/scopeMaps@2022-12-01' = {
  parent: containerRegistry
  name: '_repositories_push'
  properties: {
    description: 'Can push to any repository of the registry'
    actions: [
      'repositories/*/content/read'
      'repositories/*/content/write'
    ]
  }
}

// Outputs
output name string = containerRegistry.name
output loginServer string = containerRegistry.properties.loginServer
