metadata description = 'Creates an Azure Container Registry'

// Parameters
@description('The location for the container registry')
param location string = resourceGroup().location

@description('Specifies the environment for resources.')
@allowed([
  'dev'
  'test'
  'qa'
  'stage'
  'prod'
])
param environment string = 'dev'

@description('Specifies the name of the solution (including a probably suffix) - it is used as part of the ACR name.')
@maxLength(40)
@minLength(3)
param solutionName string

// Resources
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2022-12-01' = {
  name: '${toLower(solutionName)}${environment}'
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

// Outputs
output name string = containerRegistry.name
output loginServer string = containerRegistry.properties.loginServer
