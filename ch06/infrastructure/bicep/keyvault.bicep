/*
* Codebreaker KeyVault
*/

targetScope = 'resourceGroup'

@description('The name of the key vault.')
param name string

@description('The location where the key vault is going to be created.')
param location string

@description('The AAD principal ids of the AAD users or groups who are to be given administrator privileges for the key vault.')
param adminPrincipalIds string[]

@description('The administrator password of the SQL server, stored as secret in the key vault.')
@secure()
@minLength(10)
param sqlServerAdminPassword string

@description('The Azure Active Directory tenant ID that should be used for authenticating requests to the key vault.')
param tenantId string = subscription().tenantId

@description('The name of the secret, which stores the administrator password for the SQL server.')
param sqlServerAdminPasswordSecretName string = 'sqlAdminPassword'

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: name
  location: location
  properties: {
    enabledForTemplateDeployment: true
    enableRbacAuthorization: true
    tenantId: tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
    accessPolicies: []
  }
}

resource sqlServerAdminPasswordSecret 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  parent: keyVault
  name: sqlServerAdminPasswordSecretName
  properties: {
    contentType: 'text/plain'
    value: sqlServerAdminPassword
  }
}

var roleDefinitionId = '00482a5a-887f-4fb3-b363-3b7fe8e74483' // Key Vault Administrator
resource role 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for adminPrincipalId in adminPrincipalIds: {
  scope: keyVault
  name: guid(adminPrincipalId, roleDefinitionId, resourceGroup().id)
  properties: {
    principalId: adminPrincipalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleDefinitionId) 
  }
}]
