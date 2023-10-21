metadata description = 'Creates a user managed identity.'

targetScope = 'resourceGroup'

@description('Specifies the Azure location where the app configuration store should be created.')
param location string = resourceGroup().location

@description('Specifies the name of the user managed identity to create.')
param identityName string = 'myUserManagedIdentity'

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: identityName
  location: location
}
output identityName string = identity.name
