metadata description = 'Creates an Azure Container App Environment'

// Parameters
@description('The name for the container app environment')
param name string = 'codebreakerenv'

@description('The location for the container app environment')
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

@description('The name for the log analytics workspace')
param logAnalyticsWorkspaceName string

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
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: logAnalyticsWorkspaceName
}

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: 'cae-${name}-${environment}-${regionCode}'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
  dependsOn: [
    logAnalytics
  ]
}

// Outputs
output id string = containerAppEnvironment.id
