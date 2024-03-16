# Chapter 6

## Technical requirements

See [Installation](../installation.md) on how to install Visual Studio, Docker Desktop...

With this Chapter you need to have **Docker Desktop** installed – see [Chapter 5](../ch05/Readme.md) for more information. You also need a **Microsoft Azure** subscription. You can activate Microsoft Azure for free at [https://azure.microsoft.com/free](https://azure.microsoft.com/free) which gives you an amount of about USD 200,- Azure credits that’s available for the first 30 days and several services that can be used for free for the time after.

What many developers miss: if you have a Visual Studio Professional or Enterprise subscription, you also have a free amount of Azure resources every month. You just need to active this with your [Visual Studio subscription](https://visualstudio.microsoft.com/subscriptions/).

To work through the samples of this Chapter, besides Docker Desktop, the **Azure CLI** is needed. 
To create and manage resources, install the Azure CLI:

`winget install Microsoft.AzureCLI`

The Azure Cli is available on Mac and Linux as well. To install the Azure CLI on different platforms, see https://learn.microsoft.com/cli/azure/install-azure-cli.

An easy way to use the Azure Shell is from a Web browser. As you log into the [Azure Portal](https://portal.azure.com) at using your Microsoft Azure account, on the top button bar you’ll see an icon for the Cloud Shell. Clicking on this button, a terminal opens. Here, the Azure CLI is already installed – along with many other tools such as `wget`` to download files, git to work with repositories, docker, the .NET CLI, and more. You can also use a Visual Studio Code editor (just run `code`` from the terminal) to edit files. All the files you create and change are persisted within an *Azure Storage* account that is automatically created when you start the Cloud Shell. For a full-screen Cloud Shell, you can open https://shell.azure.com.

In this Chapter we also use **Bicep** scripts to create Azure resources. Coding Bicep scripts is best done using Visual Studio Code with the Bicep extension.

## Creating Bicep files

To create Bicep files using **azd**, you need to turn on this alpha-feature:

```bash
azd config set alpha.infraSynth on
```