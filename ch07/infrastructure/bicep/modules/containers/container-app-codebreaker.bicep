metadata description = 'Creates an Azure Container App'

// Parameters
// @description('The id of the container app environment')
// param containerAppEnvironmentId string

param containerAppEnvironment string

@description('The name for the container app')
@maxLength(16)
param name string

@description('Specifies the container image to deploy for the container app\nExample: \'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest\'')
param containerImage string = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'

@description('The location for the container app')
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

@secure()
param registryPassword string

param registryUsername string

param registryServer string

@description('The target port for the container app')
param targetPort int = 80

@description('Number of CPU cores the container can use. Can be with a maximum of two decimals.')
param cpu string = '0.25'

@description('Amount of memory (in gibibytes, GiB) allocated to the container up to 4GiB. Can be with a maximum of two decimals. Ratio with CPU cores must be equal to 2.')
param memory string = '0.5'

@description('Minimum number of replicas the container app will be deployed')
@minValue(0)
@maxValue(300)
param minReplicas int = 1

@description('Maximum number of replicas the container app will be deployed')
@minValue(1)
@maxValue(300)
param maxReplicas int = 3

// @description('user for ACR')
// param registryUser string

// @description('ACR server')
// param registryServer string

// @description('ACR password')
// @secure()
// param registryPassword1 string

// @description('The name of the container registry')
// param registry string

@description('Environment variables for the container app')
param envVars array = []

var regionCodes = {
  centralus: 'cus'
  southcentralus: 'scus'
  northeurope: 'northeu'
  westeurope: 'westeu'
  australiacentral: 'auc'
  southeastasia: 'seasia'
}

// remove space and make sure all lower case
var sanitizedLocation = toLower(replace(location, ' ', ''))

// get the region code
var regionCode = contains(regionCodes, sanitizedLocation) ? regionCodes[sanitizedLocation] : sanitizedLocation

// resource containerRegistry 'Microsoft.ContainerRegistry/registries@2022-12-01' existing = {
//     name: registry
// }

// var registryUsername = containerRegistry.listCredentials().username
// var registryPassword = containerRegistry.listCredentials().passwords[0].value
// var registryServer = containerRegistry.properties.loginServer

// Resources
resource containerAppEnv 'Microsoft.App/managedEnvironments@2023-05-01' existing = {
    name: containerAppEnvironment
}

var containerAppEnvironmentId = containerAppEnv.id

resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: '${toLower(name)}-${regionCode}-${environment}'
  location: location
  properties: {
    managedEnvironmentId: containerAppEnvironmentId
    configuration: {
      ingress: {
        external: true
        targetPort: targetPort
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      secrets: [
        {
          name: 'registry-password'
          value: registryPassword
        }
      ]
      registries: [
        {
          server: registryServer
          username: registryUsername
          passwordSecretRef: 'registry-password'
          identity: ''
        }
      ]
    }
    template: {
      containers: [
        {
          name: toLower(name)
          image: containerImage
          env: envVars
          resources: {
            cpu: json(cpu)  // Convert cpu to a number with the json() function
            memory: '${memory}Gi'
          }
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}

// Outputs
output name string = containerApp.name
