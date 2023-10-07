using 'codebreaker-apps.bicep'

param appEnvironment = 'cae-codebreakerenv-dev-westeu'
param registry = 'codebreakerwv7es46wf7tpodev.azurecr.io'
param botImage = 'codebreakerwv7es46wf7tpodev.azurecr.io/codebreaker/bot:latest'
param gamesAPIImage = 'codebreakerwv7es46wf7tpodev.azurecr.io/codebreaker/gamesapi:latest'

param botEnvVars = [
                        {
                            name: 'ApiBase'
                            value: 'https://cae-codebreaker-gamesapi-3.politepond-d88032f6.westeurope.azurecontainerapps.io'
                        }
                   ]
