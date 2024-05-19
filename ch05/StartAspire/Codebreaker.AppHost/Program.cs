var builder = DistributedApplication.CreateBuilder(args);

// TODO 1: get the SQL Server password from user secrets

// TODO 2: configure a SQL Server container with a named volume

// TODO 3: configure the Game APIs project using the SQL Server container

// TODO 5: configure the Bot project using the Games API service

builder.Build().Run();
