/*
* Log Analytics Workspace
* used by ContainerAppEnvironment
*/

// Parameters
@description('The name for the log analytics workspace')
param name string

@description('The location for the log analytics workspace')
param location string = resourceGroup().location

// Resources
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: name
  location: location
  properties: {
    sku: {
      name: 'pergb2018'
    }
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
    workspaceCapping: {
      dailyQuotaGb: -1
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// Outputs
output name string = logAnalyticsWorkspace.name
output id string = logAnalyticsWorkspace.id
