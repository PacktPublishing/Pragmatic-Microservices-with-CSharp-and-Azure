using 'container-app.bicep'

param name = 'gamesapi-v3'
param containerAppEnvironmentName = 'cae-codebreakerenv-dev-westeu'
param containerImage = 'codebreakerp5d6qfdaq37hedev.azurecr.io/codebreaker/gamesapi:latest'
param location = 'westeurope'
param environment = 'dev'
param minReplicas = 1
param maxReplicas = 3
