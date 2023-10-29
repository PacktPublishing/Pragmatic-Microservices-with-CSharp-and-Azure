# Chapter 8

## Technical Requirements

With this chapter, similar to the previous chapter, you need an Azure subscription. You also need your own GitHub repository to store secrets, create environments, and run GitHub actions. These features are available with public repositories. In case you create a private repository, the GitHub Team feature is required for creating environments (see https://github.com/pricing). You can fork the repository of the book and create GitHub actions within your fork, or create a new repository.

The code for this chapter can be found in the following GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure.

In the ch08 folder, you’ll see these projects with the final result of this chapter:

* Codebreaker.GameAPIs – the games API project we used in the previous chapter is enhanced using feature flags.
* Codebreaker.Bot - this is the implementation of the bot service which plays games.
* Codebreaker.GameAPIs.KiotaClient – this is the client library we created in Chapter 4 to be used by clients.
* Bicep – these are the Bicep scripts to create the Azure resources used in this chapter. 
* Workflows – this folder is new. Here you find all the GitHub workflows, but these don’t become active until you copy them to the .github/workflows folder in your repository.

Working through the code with this chapter, you can start using the service and bot projects, and the bicep files from the previous chapter, and the Kiota library from chapter 4.
