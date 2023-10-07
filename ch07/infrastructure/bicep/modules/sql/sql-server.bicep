/*
* SQL-Server
*/

// Parameters
@description('Specifies the location for resources.')
param location string = resourceGroup().location

@description('Specifies the name of the database server.')
param databaseServerName string = 'codebreaker-db-server'

@description('Specifies whether the database server should allow public network access or not.')
param publicNetworkAccess bool = false

@description('Administrator username for the server. Once created it cannot be changed.')
param administratorLogin string

@description('The administrator login password (required for server creation).')
@secure()
param administratorLoginPassword string

// Resources
resource sqlServer 'Microsoft.Sql/servers@2022-11-01-preview' = {
  name: toLower(databaseServerName)
  location: location
  properties: {
    publicNetworkAccess: publicNetworkAccess ? 'Enabled' : 'Disabled'
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
  }
}

// Outputs
output serverName string = sqlServer.name
