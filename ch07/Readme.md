# Chapter 7

## Technical Requirements

Similar to the previous Chapter, here an Azure subscription and installation of Docker Desktop is required. See the technical requirements from the previous Chapter for more information.

The code for this chapter can be found in the following GitHub repository: https://github.com/PacktPublishing/Pragmatic-Microservices-With-CSharp-and-Azure

In the ch07 folder you’ll see these projects with the final result of this chapter:

* ConfigurationPrototype – this is a new project that shows some concepts with configuration before implementing this with the games API and the bot service.
* Codebreaker.GameAPIs – the games API project we used in the previous chapter is enhanced using Azure App Configuration. Instead of having the projects with the database access code and the models as part of this Chapter, now NuGet packages are referenced.
* Codebreaker.Bot - this is the implementation of the bot service which plays games. This project is enhanced with Azure App Configuration is well.
* Bicep – here are the Bicep scripts to create the Azure resources used with this Chapter

You can start with the results from the previous chapter to work on your own through this chapter.
