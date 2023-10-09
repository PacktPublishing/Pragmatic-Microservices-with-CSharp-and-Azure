metadata description = 'Creates a Cosmos DB account and a database.'

targetScope = 'resourceGroup'

// Parameters
@description('Specifies the location for resources.')
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

@description('Specifies the name of the solution (including a possible suffix) - it is used as part of the database account name.')
@maxLength(35)
param solutionName string

@description('Specifies the name of the database, default the name of the solution')
param databaseName string = solutionName

@description('Whether the free-tier should be enabled or not.')
param freeTier bool = true

// Resources
resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: 'cosmos-${toLower(solutionName)}-${environment}'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    enableFreeTier: freeTier
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
      maxIntervalInSeconds: 5
      maxStalenessPrefix: 100
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  parent: databaseAccount
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
  }
}

// Outputs
output databaseAccountName string = databaseAccount.name
output databaseName string = database.name
