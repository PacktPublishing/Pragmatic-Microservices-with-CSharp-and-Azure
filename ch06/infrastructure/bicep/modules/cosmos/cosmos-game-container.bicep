/*
* Cosmos Game Container
*/

// Parameters
@description('Specifies the name of the database account.')
param databaseAccountName string

@description('Specifies the name of the database.')
param databaseName string

@description('Specifies the name of the games-container.')
param gamesContainerName string = 'Games'

@description('Specifies the throughput of the games container.')
param gamesThroughput int = 400

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
      throughput: gamesThroughput
      // OR
      // autoscaleSettings: {
      //   maxThroughput: gamesThroughput
      // }
    }
  }
}

// Outputs
output id string = gameContainer.id
