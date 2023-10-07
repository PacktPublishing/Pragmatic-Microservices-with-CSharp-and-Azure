metadata description = 'Creates Azure Container Apps for the Games API and the Bot service'

targetScope = 'resourceGroup'
var resourceGroup = az.resourceGroup()

// Parameters
@description('Specifies the environment for resources.')
@allowed([
  'dev'
  'test'
  'qa'
  'stage'
  'prod'
])
param environment string = 'dev'

param registry string

param botImage string
param gamesAPIImage string

param botEnvVars array

@description('Specifies the location used for the resources. Default is the location of the resource.')
param location string = az.resourceGroup().location


@description('A string placed after the name of resources, which require a unique name.\nDefault: Unique string based on the resource-id.')
param nameSuffix string = uniqueString(az.resourceGroup().id)
var dashNameSuffix = '${nameSuffix != '' ? '-' : null}${nameSuffix}' // Prepend a dash, if the nameSuffix is not empty

@description('The Azure App Container Environment')
param appEnvironment string

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' existing = {
  name: appEnvironment
}



// Modules

module gamesAPIcontainerAppModule 'modules/containers/container-app-codebreaker.bicep' = {
  name: 'gamesAPI'
  dependsOn: [ containerAppEnvironment ]
  scope: resourceGroup
  params: {
    containerImage: gamesAPIImage
    containerAppEnvironmentId: containerAppEnvironment.id
    registry: registry
    name: 'gamesAPI'
    location: location
    cpu: '0.25'
    memory: '0.5'
    targetPort: 8080
    minReplicas: 1
    maxReplicas: 3
  }
}

module botContainerAppModule 'modules/containers/container-app.bicep' = {
  name: 'bot'
  dependsOn: [ containerAppEnvironment ]
  scope: resourceGroup
  params: {
    containerImage: botImage
    containerAppEnvironmentId: containerAppEnvironment.id
    name: 'bot'
    envVars: botEnvVars
    location: location
    cpu: '0.25'
    memory: '0.5'
    targetPort: 8080
    minReplicas: 0
    maxReplicas: 3
  }
}
