@description('The Azure Cosmos DB account name.')
@maxLength(44)
param databaseAccountName string

@description('The id of the cosmos role definition to assign.')
param cosmosRoleId string

@description('The id of the principal to whom the role is to be assigned.')
param principalId string

resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' existing = {
  name: toLower(databaseAccountName)
}

resource cosmosDataContributorRole 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2023-04-15' existing = {
  parent: databaseAccount
  name: cosmosRoleId
}

resource gameApiContainerAppComsosDataContributorRoleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2023-04-15' = {
  parent: databaseAccount
  name: guid(databaseAccount.id, principalId)
  properties: {
    roleDefinitionId: cosmosDataContributorRole.id
    principalId: principalId
    scope: databaseAccount.id
  }
}
