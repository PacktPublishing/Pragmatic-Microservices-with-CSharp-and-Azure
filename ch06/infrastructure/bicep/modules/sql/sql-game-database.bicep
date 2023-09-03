/*
* SQL Game Database
*/

// Parameters
@description('Specifies the location for resources.')
param location string = resourceGroup().location

@description('Specifies the name of the database server.')
param databaseServerName string = 'codebreaker-db-server'

@description('Specifies the name of the database.')
param databaseName string = 'codebreaker-db'

@description('The max size of the database expressed in bytes.\nKeep the limitations of your selected tier in mind.')
@minValue(536870912) // 0.5 GibiBytes
param maxSizeBytes int = 2147483648 // 2 GibiBytes

@description('Time in minutes after which database is automatically paused. A value of -1 means that automatic pause is disabled.')
@minValue(-1)    // disabled
@maxValue(10080) // 7 days
param autoPauseDelay int = -1

@allowed(['DTU', 'vCore'])
param sku string = 'DTU'

@description('Specifies the capacity of the database.\nNote the restrictions of the respective SKU.')
@minValue(1)
@maxValue(80)
param capacity int = 5

@description('Minimal capacity that database will always have allocated, if not paused. Only if "sku" is vCore serverless.')
@minValue(0)
@maxValue(80)
param minCapacity int = 0

var skus = {
  DTU: {
    name: 'Basic'
    tier: 'Basic'
    capacity: capacity
  }
  vCore: {
    name: 'GP_5_Gen5'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: capacity
  }
}

// Resources
resource sqlServer 'Microsoft.Sql/servers@2022-11-01-preview' existing = {
  name: databaseServerName
}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2022-11-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  sku: skus[sku]
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    autoPauseDelay: autoPauseDelay
    maxSizeBytes: maxSizeBytes
    minCapacity: minCapacity
  }
}

// Outputs
output name string = sqlServerDatabase.name
