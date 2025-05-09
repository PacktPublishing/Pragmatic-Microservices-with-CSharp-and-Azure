targetScope = 'subscription'

param environmentName string

param location string

param principalId string

var tags = {
  'aspire-env-name': environmentName
}

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}

module codebreakercosmos 'codebreakercosmos/codebreakercosmos.bicep' = {
  name: 'codebreakercosmos'
  scope: rg
  params: {
    location: location
  }
}

module codebreakercosmos_roles 'codebreakercosmos-roles/codebreakercosmos-roles.bicep' = {
  name: 'codebreakercosmos-roles'
  scope: rg
  params: {
    location: location
    codebreakercosmos_outputs_name: codebreakercosmos.outputs.name
    principalId: ''
  }
}