# Test with Playwright

> The Playwright tests are available in chapter 10 and 12.

[Deploying the Application to Azure using azd](Deploy2Azure.md)

## Run the load tests

1. Change to the directory of the project *Codebreaker.GamesAPI.Playwright*:

```bash
cd Codebreaker.GameAPIs.Playwright
```

2. Change the `BaseUrl` configuration to reference your Azure Container App games API (appsettings.json)
3. Build the project `dotnet build`
4. Install required browsers:

```powershell
pwsh bin/Debug/net8.0/playwright.ps1 install
```

5. Get the access token from the [Playwright portal](https://aka.ms/mpt/portal), and set the environment variable for your project: 

 ```powershell
    $env:PLAYWRIGHT_SERVICE_ACCESS_TOKEN= # Paste Access Token value from previous step
 ```
    
6. In the [Playwright portal](https://aka.ms/mpt/portal), copy the command under **Add region endpoint in your set up** and set the following environment variable:

```powershell
$env:PLAYWRIGHT_SERVICE_URL= # Paste region endpoint URL
```

7. Start the tests

```powershell
dotnet test -- NUnit.NumberOfTestWorkers=50
```
