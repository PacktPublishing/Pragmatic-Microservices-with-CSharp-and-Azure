using 'container-app.bicep'

param name = 'bot-v3'
param containerAppEnvironmentName = 'cae-codebreakerenv-dev-westeu'
param containerImage = 'codebreakerp5d6qfdaq37hedev.azurecr.io/codebreaker/bot:latest'
param location = 'westeurope'
param environment = 'dev'
param minReplicas = 0
param maxReplicas = 1
