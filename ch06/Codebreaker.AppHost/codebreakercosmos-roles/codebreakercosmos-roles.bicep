@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param codebreakercosmos_outputs_name string

param principalId string

resource codebreakercosmos 'Microsoft.DocumentDB/databaseAccounts@2024-08-15' existing = {
  name: codebreakercosmos_outputs_name
}

resource codebreakercosmos_roleDefinition 'Microsoft.DocumentDB/databaseAccounts/sqlRoleDefinitions@2024-08-15' existing = {
  name: '00000000-0000-0000-0000-000000000002'
  parent: codebreakercosmos
}

resource codebreakercosmos_roleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2024-08-15' = {
  name: guid(principalId, codebreakercosmos_roleDefinition.id, codebreakercosmos.id)
  properties: {
    principalId: principalId
    roleDefinitionId: codebreakercosmos_roleDefinition.id
    scope: codebreakercosmos.id
  }
  parent: codebreakercosmos
}