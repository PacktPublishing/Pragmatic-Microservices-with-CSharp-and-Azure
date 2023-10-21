using 'container-app.bicep'

param name = 'bot-v3'
param containerAppEnvironmentName = 'cae-codebreakerenv-dev-westeu'
param containerImage = 'codebreaker2j65mk73a46zmdev.azurecr.io/codebreaker/bot:latest'
param registryServer = 'codebreaker2j65mk73a46zmdev.azurecr.io'
param location = 'westeurope'
param identityName = 'id-codebreaker-dev'
param environment = 'dev'
param minReplicas = 0
param maxReplicas = 1
