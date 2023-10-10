metadata description = 'Creates a Cosmos DB account, database, and container with permissions to read and write to the container.'

targetScope = 'resourceGroup'

// Parameters
@description('Specifies the name of the existing database account name.')
param databaseAccountName string

@description('Specifies the principal id (GUID) for the managed identity.')
param principalId string

var roleDefinitionId = guid('sql-role-definition-', principalId, databaseAccount.id)
var roleAssignmentId = guid(roleDefinitionId, principalId, databaseAccount.id)

// Resources
resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2023-09-15' existing = {
  name: databaseAccountName
}

resource sqlRoleDefinition 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2021-04-15' = {
  name: roleDefinitionId
  parent: databaseAccount
  properties: {
    roleName: 'Codebreaker Read-Write Role'
    type: 'CustomRole'
    assignableScopes: [
      databaseAccount.id
    ]
    permissions: [
      {
        dataActions: [
          'Microsoft.DocumentDB/databaseAccounts/readMetadata'
          'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*'
        ]
      }
    ]
  }
}

resource sqlRoleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2021-04-15' = {
  name: roleAssignmentId
  parent: databaseAccount
  properties: {
    principalId: principalId
    roleDefinitionId: sqlRoleDefinition.id
    scope: databaseAccount.id 
  }
}

// Outputs
output sqlRoleAssignmentId string = sqlRoleAssignment.id
output sqlRoleDefinitionId string = sqlRoleDefinition.id
