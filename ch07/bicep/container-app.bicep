metadata description = 'Creates an Azure Container App'

// Parameters
@description('The name of the container app environment')
param containerAppEnvironmentName string

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

@description('The target port for the container app')
param targetPort int = 8080

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

// Resources

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' existing = {
  name: containerAppEnvironmentName
}

resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: '${name}-${regionCode}-${environment}'
  location: location
  properties: {
    managedEnvironmentId: containerAppEnvironment.id
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
    }
    template: {
      containers: [
        {
          name: name
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
