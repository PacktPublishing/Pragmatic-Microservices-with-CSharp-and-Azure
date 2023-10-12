metadata description = 'Creates the role definition and role assignments to read and write from the Azure Cosmos DB database.'

targetScope = 'resourceGroup'

// Parameters
@description('Specifies the name of the existing database account name.')
param databaseAccountName string

@description('The managed identity to get access to the key vault secrets.')
param managedIdentityName string

// Resources
resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2023-09-15' existing = {
  name: databaseAccountName
}

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' existing = {
  name: managedIdentityName
}

var roleDefinitionId = guid('sql-role-definition-', managedIdentity.name, databaseAccount.id)
var roleAssignmentId = guid(roleDefinitionId, managedIdentity.name, databaseAccount.id)

resource cosmosReadWriteRoleDefinition 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2023-09-15' = {
  name: roleDefinitionId
  parent: databaseAccount
  properties: {
    roleName: 'Cosmos Read-Write Role'
    type: 'CustomRole'
    assignableScopes: [
      databaseAccount.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/*'
        ]
        notDataActions: []
      }
    ]
  }
}

resource cosmosRoleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2023-09-15' = {
  name: roleAssignmentId
  parent: databaseAccount
  properties: {
    principalId: managedIdentity.properties.principalId
    roleDefinitionId: cosmosReadWriteRoleDefinition.id
    scope: databaseAccount.id 
  }
}

// Outputs
output sqlRoleAssignmentId string = cosmosRoleAssignment.id
output sqlRoleDefinitionId string = cosmosReadWriteRoleDefinition.id
