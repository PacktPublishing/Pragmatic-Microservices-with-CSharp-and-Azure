metadata description = 'Creates a Log analytics workspace'

// Parameters
@description('The name for the solution used by the log analytics workspace')
param solutionName string

@description('The location for the log analytics workspace')
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

var regionCodes = {
  centralus: 'cus'
  southcentralus: 'scus'
  northeurope: 'northeu'
  westeurope: 'westeu'
  australiacentral: 'auc'
  southeastasia: 'seasia'
}

// remove spaces and make sure all lower case
var sanitizedLocation = toLower(replace(location, ' ', ''))

// get the region code
var regionCode = contains(regionCodes, sanitizedLocation) ? regionCodes[sanitizedLocation] : sanitizedLocation

// Resources
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: 'logs-${solutionName}-${environment}-${regionCode}'
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
