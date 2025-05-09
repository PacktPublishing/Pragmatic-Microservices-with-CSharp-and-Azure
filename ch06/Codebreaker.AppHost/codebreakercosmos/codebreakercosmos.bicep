@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource codebreakercosmos 'Microsoft.DocumentDB/databaseAccounts@2024-08-15' = {
  name: take('codebreakercosmos-${uniqueString(resourceGroup().id)}', 44)
  location: location
  properties: {
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    databaseAccountOfferType: 'Standard'
    disableLocalAuth: true
  }
  kind: 'GlobalDocumentDB'
  tags: {
    'aspire-resource-name': 'codebreakercosmos'
  }
}

resource codebreaker 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-08-15' = {
  name: 'codebreaker'
  location: location
  properties: {
    resource: {
      id: 'codebreaker'
    }
  }
  parent: codebreakercosmos
}

resource GamesV3 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-08-15' = {
  name: 'GamesV3'
  location: location
  properties: {
    resource: {
      id: 'GamesV3'
      partitionKey: {
        paths: [
          '/PartitionKey'
        ]
      }
    }
  }
  parent: codebreaker
}

output connectionString string = codebreakercosmos.properties.documentEndpoint

output name string = codebreakercosmos.name