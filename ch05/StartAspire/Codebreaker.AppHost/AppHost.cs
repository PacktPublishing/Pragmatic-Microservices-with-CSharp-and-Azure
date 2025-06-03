#pragma warning disable ASPIRECOSMOSDB001

var builder = DistributedApplication.CreateBuilder(args);

// TODO 6: see updates.md file to publish as Docker Compose

// TODO 1: configure a SQL Server container with a named volume

// TODO 2: configure the Game APIs project using the SQL Server container

// TODO 4: configure the Bot project using the Games API service

builder.Build().Run();
