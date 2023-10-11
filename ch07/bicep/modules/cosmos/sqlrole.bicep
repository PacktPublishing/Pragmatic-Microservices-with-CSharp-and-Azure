metadata description = 'Creates the role definition and role assignments to read and write from the Azure Cosmos DB database.'

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

resource sqlRoleDefinition 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2023-09-15' = {
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

resource sqlRoleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2023-09-15' = {
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
