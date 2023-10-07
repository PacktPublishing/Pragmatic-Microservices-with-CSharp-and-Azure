/*
* Cosmos DatabaseAccount and Cosmos Database
*/

// Parameters
@description('Specifies the location for resources.')
param location string = resourceGroup().location

@description('Specifies the name of the database account.')
param databaseAccountName string

@description('Specifies the name of the database.')
param databaseName string

@description('The AAD-PrincipalIds of the developers, needing read-write access to the database')
param userPrincipalIds string[]

@description('Whether the free-tier should be enabled or not.')
param freeTier bool = false

// Resources
resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: toLower(databaseAccountName)
  location: location
  kind: 'GlobalDocumentDB'
  identity: {
    type: 'SystemAssigned'
  }
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

resource cosmosDataReaderRole 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2023-04-15' = {
  parent: databaseAccount
  name: '00000000-0000-0000-0000-000000000001'
  properties: {
    roleName: 'Cosmos DB Built-in Data Reader'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccount.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/executeQuery'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/readChangeFeed'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/read'
        ]
        notDataActions: []
      }
    ]
  }
}

resource cosmosDataContributorRole 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2023-04-15' = {
  parent: databaseAccount
  name: '00000000-0000-0000-0000-000000000002'
  properties: {
    roleName: 'Cosmos DB Built-in Data Contributor'
    type: 'BuiltInRole'
    assignableScopes: [
      databaseAccount.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/*'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*'
        ]
        notDataActions: []
      }
    ]
  }
}

resource userRoleAssignments 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2023-04-15' = [for principalId in userPrincipalIds: {
  parent: databaseAccount
  name: principalId
  properties: {
    roleDefinitionId: cosmosDataContributorRole.id
    principalId: principalId
    scope: databaseAccount.id
  }
}]

// Outputs
output databaseAccountName string = databaseAccount.name
output databaseId string = database.name
