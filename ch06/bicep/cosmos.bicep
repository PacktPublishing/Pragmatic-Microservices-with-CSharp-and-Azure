metadata description = 'Creates a Cosmos DB account, database, and container.'

targetScope = 'resourceGroup'

// Parameters
@description('Specifies the name of the database account without prefix and suffix.')
param location string = resourceGroup().location

@description('Specifies the environment for resources.')
@allowed([
  'dev'
  'test'
  'prod'
])
param environment string = 'dev'

@description('Specifies the name of the solution - is used as part of the database account name.')
@maxLength(20)
param solutionName string

@description('Specifies the name of the database.')
param databaseName string

@description('Specifies the name of the games container.')
param gamesContainerName string

@description('Whether the free-tier should be enabled or not.')
param freeTier bool = true

// Resources
resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: 'cosmos-${toLower(solutionName)}-${uniqueString(resourceGroup().id)}-${environment}'
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

resource gameContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-04-15' = {
  parent: database
  name: gamesContainerName
  properties: {
    resource: {
      id: gamesContainerName
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
      partitionKey: {
        paths: [
          '/PartitionKey'
        ]
        kind: 'Hash'
        version: 2
      }
      uniqueKeyPolicy: {
        uniqueKeys: []
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
    }
  }
}

// Outputs
output databaseAccountName string = databaseAccount.name
output databaseName string = database.name
output gamesContainerName string = gameContainer.name
