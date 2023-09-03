/*
* Container App Environment
*/

// Parameters
@description('The name for the container app environment')
param name string = 'codebreakerenv'

@description('The location for the container app environment')
param location string = resourceGroup().location

@description('The name for the log analytics workspace')
param logAnalyticsWorkspaceName string

// Resources
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: logAnalyticsWorkspaceName
}

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: name
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
