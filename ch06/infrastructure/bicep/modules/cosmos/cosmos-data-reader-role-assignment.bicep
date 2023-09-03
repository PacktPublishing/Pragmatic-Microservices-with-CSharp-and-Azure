@description('The Azure Cosmos DB account name.')
@maxLength(44)
param databaseAccountName string

@description('The id of the principal to whom the role is to be assigned.')
param principalId string

module roleAssignment 'cosmos-role-assignment.bicep' = {
  name: 'role-assignment'
  params: {
    databaseAccountName: databaseAccountName
    principalId: principalId
    cosmosRoleId: '00000000-0000-0000-0000-000000000001'
  }
}
