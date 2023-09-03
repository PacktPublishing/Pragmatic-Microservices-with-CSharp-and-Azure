using 'codebreaker.bicep'

param location = 'West Europe'
param sqlAdminPassword = null     // Must only be specified, if no key vault gets used
param sqlAdminPasswordSecret = {
  keyVaultName: '' // <- specify the name of the keyvault here
  sqlAdminPasswortSecretName: 'sqlAdminPassword'  // The name of the secret in the key vault
}
param developerPrincipleIds = [] // You can get your own principalId by running the Azure CLI command  `az ad signed-in-user show --query id`
