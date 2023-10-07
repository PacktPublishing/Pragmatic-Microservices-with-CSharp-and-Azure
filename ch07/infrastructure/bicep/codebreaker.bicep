/*
* Codebreaker Resource Group
*/

targetScope = 'resourceGroup'
var resourceGroup = az.resourceGroup()

// Parameters
@description('Specifies the location used for the resources.')
param location string

@description('The administrator password for the SQL server.\nEither specify this parameter OR the parameter "sqlAdminPasswordSecret" if the password is stored as secret in the keyVault.')
@secure()
param sqlAdminPassword string?

@description('The required parameters to access the keyvault for the the SQL administrator password.')
param sqlAdminPasswordSecret {
  keyVaultName: string
  sqlAdminPasswortSecretName: string
}?

@description('A string placed after the name of resources, which require a unique name across subscriptions.\nDefault: Unique string based on the subscription-id.')
param nameSuffix string = uniqueString(subscription().subscriptionId)
var dashNameSuffix = '${nameSuffix != '' ? '-' : null}${nameSuffix}' // Prepend a dash, if the nameSuffix is not empty

@description('Specifies the principal ids of the developers, needing AAD access to some resources (e.g. Cosmos database account).')
param developerPrincipleIds string[]

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = if (sqlAdminPasswordSecret != null) {
  name: sqlAdminPasswordSecret!.keyVaultName
}

// Modules
module logAnalyticsWorkspace 'modules/misc/log-analytics-workspace.bicep' = {
  name: 'log-analytics-workspace'
  scope: resourceGroup
  params: {
    name: 'codebreakerlogs'
    location: location
  }
}

module containerRegistry 'modules/containers/container-registry.bicep' = {
  name: 'container-registry'
  scope: resourceGroup
  params: {
    name: 'codebreaker${nameSuffix}'
    location: location
  }
}

var cosmosDatabaseAccountName = 'codebreaker-cosmos${dashNameSuffix}'
var cosmosDatabaseName = 'codebreaker'
module cosmos 'modules/cosmos/cosmos.bicep' = {
  name: 'cosmos'
  scope: resourceGroup
  params: {
    databaseAccountName: cosmosDatabaseAccountName
    location: location
    freeTier: false
    userPrincipalIds: developerPrincipleIds
    databaseName: cosmosDatabaseName
  }
}

module cosmosGameContainer 'modules/cosmos/cosmos-game-container.bicep' = {
  name: 'cosmos-games'
  dependsOn: [ cosmos ]
  scope: resourceGroup
  params: {
    databaseAccountName: cosmosDatabaseAccountName
    databaseName: cosmosDatabaseName
  }
}

var sqlServerName = 'codebreaker-sql-server${dashNameSuffix}'
module sql 'modules/sql/sql-server.bicep' = {
  name: 'sql'
  scope: resourceGroup
  params: {
    databaseServerName: sqlServerName
    location: location
    publicNetworkAccess: true
    administratorLogin: 'sqladmin'
    administratorLoginPassword: sqlAdminPassword != null ? sqlAdminPassword! : keyVault.getSecret(sqlAdminPasswordSecret!.sqlAdminPasswortSecretName)
  }
}

module sqlGameDb 'modules/sql/sql-game-database.bicep' = {
  name: 'sql-games'
  dependsOn: [ sql ]
  scope: resourceGroup
  params: {
    location: location
    databaseServerName: sqlServerName
    maxSizeBytes: 2147483648
  }
}

module containerAppEnvironment 'modules/containers/container-app-environment.bicep' = {
  name: 'container-app-environment'
  scope: resourceGroup
  params: {
    name: 'codebreakerenv'
    location: location
    logAnalyticsWorkspaceName:logAnalyticsWorkspace.outputs.name
  }
}

module gameApiContainerApp 'modules/containers/game-api-container-app.bicep' = {
  name: 'codebreaker-gamesapi'
  dependsOn: [ cosmosGameContainer ]
  scope: resourceGroup
  params: {
    containerAppEnvironmentId: containerAppEnvironment.outputs.id
    name: 'codebreaker-gamesapi'
    location: location
    databaseAccountName: cosmosDatabaseAccountName
    cpu: '0.5'
    memory: '1'
    port: 8080
    minReplicas: 1
    maxReplicas: 3
  }
}
