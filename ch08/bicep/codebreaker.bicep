metadata description = 'Creates Azure resources for the Codebreaker solution'

targetScope = 'resourceGroup'
var resourceGroup = az.resourceGroup()

// Parameters
@description('Specifies the environment for resources.')
@allowed([
  'dev'
  'test'
  'qa'
  'stage'
  'prod'
])
param environment string = 'dev'

@description('Specifies the location used for the resources. Default is the location of the resource.')
param location string = az.resourceGroup().location

@description('Specifies the name of the solution - it is used as part of the Azure resource names.')
@maxLength(20)
param solutionName string

@description('Should the free-tier for Cosmos be used?')
param cosmosFreeTier bool = true

@description('A string placed after the name of resources, which require a unique name.\nDefault: Unique string based on the resource-id.')
param nameSuffix string = uniqueString(az.resourceGroup().id)
var dashNameSuffix = '${nameSuffix != '' ? '-' : null}${nameSuffix}' // Prepend a dash, if the nameSuffix is not empty

// Modules
module managedIdentityModule 'modules/managedidentity/managedidentity.bicep' = {
  name: 'managed-identity'
  scope: resourceGroup
  params: {
    identityName: 'id-codebreaker-${environment}'
    location: location
  }
}

module logAnalyticsWorkspaceModule 'modules/loganalytics/log-analytics-workspace.bicep' = {
  name: 'log-analytics-workspace'
  scope: resourceGroup
  params: {
    location: location
    environment: environment
    solutionName: solutionName
  }
}

module containerRegistry 'modules/containers/container-registry.bicep' = {
  name: 'container-registry'
  scope: resourceGroup
  params: {
    solutionName: '${solutionName}${nameSuffix}' // dash is not allowed with the ACR name
    environment: environment
    location: location
  }
}

module cosmosModule 'modules/cosmos/cosmos.bicep' = {
  name: 'cosmos-accountanddatabase'
  scope: resourceGroup
  params: {
    solutionName: '${solutionName}${dashNameSuffix}'
    databaseName: 'codebreaker'
    location: location
    environment: environment
    freeTier: cosmosFreeTier
  }
}

module cosmosGameContainerModule 'modules/cosmos/cosmos-container.bicep' = {
  name: 'cosmos-gamescontainer'
  dependsOn: [ cosmosModule ]
  scope: resourceGroup
  params: {
    gamesContainerName: 'GamesV3'
    databaseAccountName: cosmosModule.outputs.databaseAccountName
    databaseName: cosmosModule.outputs.databaseName
  }
}

module sqlRoleModule 'modules/cosmos/sqlrole.bicep' = {
  name: 'sql-role'
  dependsOn: [ cosmosModule, cosmosGameContainerModule, managedIdentityModule ]
  scope: resourceGroup
  params: {
    managedIdentityName: managedIdentityModule.outputs.identityName
    databaseAccountName: cosmosModule.outputs.databaseAccountName
  }
}

module appConfigurationModule 'modules/appconfiguration/appconfiguration.bicep' = {
  name: 'appconfiguration'
  dependsOn: [ managedIdentityModule ]
  scope: resourceGroup
  params: {
    configStoreName: 'acs-${solutionName}-${nameSuffix}-${environment}'
    location: location
    managedIdentityName: managedIdentityModule.outputs.identityName
  }
}

module keyVaultModel 'modules/keyvault/keyvault.bicep' = {
  name: 'keyvault'
  dependsOn: [ managedIdentityModule ]
  scope: resourceGroup
  params:{
    vaultName: 'kv${uniqueString((resourceGroup.id))}'
    location: location
    managedIdentityName: managedIdentityModule.outputs.identityName
  }
}

module containerAppEnvironmentModule 'modules/containers/container-app-environment.bicep' = {
  name: 'container-app-environment'
  dependsOn: [ logAnalyticsWorkspaceModule ]
  scope: resourceGroup
  params: {
    name: 'codebreakerenv'
    location: location
    environment: environment
    logAnalyticsWorkspaceName:logAnalyticsWorkspaceModule.outputs.name
  }
}

module containerAppModule 'modules/containers/container-app.bicep' = {
  name: 'hello'
  dependsOn: [ containerAppEnvironmentModule ]
  scope: resourceGroup
  params: {
    containerAppEnvironmentId: containerAppEnvironmentModule.outputs.id
    name: 'hello'
    location: location
    environment: environment
    cpu: '0.25'
    memory: '0.5'
    targetPort: 80
    minReplicas: 0
    maxReplicas: 3
  }
}
