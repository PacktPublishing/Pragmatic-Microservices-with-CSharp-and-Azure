targetScope = 'resourceGroup'

@description('')
param location string = resourceGroup().location

@description('')
param keyVaultName string


resource keyVault_IeF8jZvXV 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource cosmosDBAccount_3PK5KyY96 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: toLower(take('codebreakercosmos${uniqueString(resourceGroup().id)}', 24))
  location: location
  tags: {
    'aspire-resource-name': 'codebreakercosmos'
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

resource cosmosDBSqlDatabase_Sa75U8Y0K 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-04-15' = {
  parent: cosmosDBAccount_3PK5KyY96
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
    value: 'AccountEndpoint=${cosmosDBAccount_3PK5KyY96.properties.documentEndpoint};AccountKey=${cosmosDBAccount_3PK5KyY96.listkeys(cosmosDBAccount_3PK5KyY96.apiVersion).primaryMasterKey}'
  }
}
