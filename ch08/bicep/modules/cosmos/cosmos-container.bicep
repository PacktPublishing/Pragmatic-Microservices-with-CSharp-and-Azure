metadata description = 'Creates a Cosmos DB container.'

// Parameters
@description('Specifies the name of the database account.')
param databaseAccountName string

@description('Specifies the name of the database.')
param databaseName string

@description('Specifies the name of the container.')
param gamesContainerName string

@description('Specifies the throughput of the games container.')
@minValue(400)
@maxValue(1000000)
param containerThroughput int = 400

// Resources
resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: databaseAccountName
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' existing = {
  parent: databaseAccount
  name: databaseName
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

resource databaseThroughput 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/throughputSettings@2023-04-15' = {
  parent: gameContainer
  name: 'default'
  properties: {
    resource: {
      throughput: containerThroughput
    }
  }
}

// Outputs
output id string = gameContainer.id
