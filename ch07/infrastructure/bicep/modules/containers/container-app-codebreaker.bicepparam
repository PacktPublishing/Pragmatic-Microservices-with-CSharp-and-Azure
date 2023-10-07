using 'container-app-codebreaker.bicep'

param containerAppEnvironment = 'cae-codebreakerenv-dev-westeu'
param registryServer = 'codebreakerwv7es46wf7tpodev.azurecr.io'
// param botImage = 'codebreakerwv7es46wf7tpodev.azurecr.io/codebreaker/bot:latest'
// param gamesAPIImage = 'codebreakerwv7es46wf7tpodev.azurecr.io/codebreaker/gamesapi:latest'

// param appEnvironment = 'cae-codebreakerenv-dev-westeu'
// param registry = 'codebreakerwv7es46wf7tpodev'
param containerImage = 'codebreakerwv7es46wf7tpodev.azurecr.io/codebreaker/bot:latest'
// param gamesAPIImage = 'codebreakerwv7es46wf7tpodev.azurecr.io/codebreaker/gamesapi:latest'

param name = 'test5'

// param registry = 'codebreakerwv7es46wf7tpodev'
param registryUsername = 'codebreakerwv7es46wf7tpodev'
param registryPassword = 'codebreakerwv7es46wf7tpodev'

param envVars = [
                        {
                            name: 'ApiBase'
                            value: 'https://cae-codebreaker-gamesapi-3.politepond-d88032f6.westeurope.azurecontainerapps.io'
                        }
                   ]
