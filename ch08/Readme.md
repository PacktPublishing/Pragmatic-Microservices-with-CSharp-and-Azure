# Chapter 8

## Technical Requirements

With this chapter, similar to the previous chapter, you need an Azure subscription, .NET Aspire, the Azure CLI, the Azure Developer CLI.
You also need your own GitHub repository to store secrets, create environments, and run GitHub actions. These features are available with public repositories. In case you create a private repository, the GitHub Team feature is required for creating environments (see https://github.com/pricing). It's best to create a new GitHub repository and just copy the code from this chapter to the repository to the `src` folder.

See [Installation](../installation.md) on how to install Visual Studio, Docker Desktop...

The code for this chapter can be found in the following GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure.

In the ch08 folder, you’ll see these projects with the final result of this chapter:

* Codebreaker.GameAPIs – the games API project we used in the previous chapter is enhanced using feature flags.
* Codebreaker.Bot - this is the implementation of the bot service which plays games.
* Codebreaker.GameAPIs.KiotaClient – this is the client library we created in Chapter 4 to be used by clients.
* Workflows – this folder is new. Here you find all the GitHub workflows, but these don’t become active until you copy them to the .github/workflows folder in your repository.

Working through the code with this chapter, you can start using the service and bot projects from the previous chapter, and the Kiota library from chapter 4.

## Updates

Some Azure Developer CLI features might change. See the current state with these GitHub issues / discussions:

- [Workflow for multi-environment management + promoting releases between environments](https://github.com/Azure/azure-dev/issues/2373)
- [azd pipeline config uses GitHub repository variables instead of secrets](https://github.com/Azure/azure-dev/issues/3586)
- [UnmatchedPrincipalType in .NET Aspire application with azd provision in GitHub action](https://github.com/Azure/azure-dev/issues/3581)
- [azd pipeline config requires a github/workflows folder in the projects folder](https://github.com/Azure/azure-dev/issues/3579)
