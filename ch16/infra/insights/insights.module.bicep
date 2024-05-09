targetScope = 'resourceGroup'

@description('')
param location string = resourceGroup().location

@description('')
param applicationType string = 'web'

@description('')
param kind string = 'web'

@description('')
param logAnalyticsWorkspaceId string


resource applicationInsightsComponent_DZzQPHvSB 'Microsoft.Insights/components@2020-02-02' = {
  name: toLower(take('insights${uniqueString(resourceGroup().id)}', 24))
  location: location
  tags: {
    'aspire-resource-name': 'insights'
  }
  kind: kind
  properties: {
    Application_Type: applicationType
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}

output appInsightsConnectionString string = applicationInsightsComponent_DZzQPHvSB.properties.ConnectionString
