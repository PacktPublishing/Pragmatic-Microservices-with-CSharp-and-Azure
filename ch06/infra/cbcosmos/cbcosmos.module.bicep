targetScope = 'resourceGroup'

@description('')
param location string = resourceGroup().location

@description('')
param keyVaultName string


resource keyVault_IeF8jZvXV 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource cosmosDBAccount_PhmSbd5Pp 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: toLower(take(concat('cbcosmos', uniqueString(resourceGroup().id)), 24))
  location: location
  tags: {
    'aspire-resource-name': 'cbcosmos'
  }
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
  }
}

resource cosmosDBSqlDatabase_JeKOjZq4t 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  parent: cosmosDBAccount_PhmSbd5Pp
  name: 'codebreaker'
  location: location
  properties: {
    resource: {
      id: 'codebreaker'
    }
  }
}

resource keyVaultSecret_Ddsc3HjrA 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault_IeF8jZvXV
  name: 'connectionString'
  location: location
  properties: {
    value: 'AccountEndpoint=${cosmosDBAccount_PhmSbd5Pp.properties.documentEndpoint};AccountKey=${cosmosDBAccount_PhmSbd5Pp.listkeys(cosmosDBAccount_PhmSbd5Pp.apiVersion).primaryMasterKey}'
  }
}
