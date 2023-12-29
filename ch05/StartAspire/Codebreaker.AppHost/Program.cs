var builder = DistributedApplication.CreateBuilder(args);


// TODO: get the SQL Server password from user secrets

// TODO: configure a SQL Server container with a named volume

// TODO: configure the Game APIs project using the SQL Server container

// TODO: configure the Bot project using the Games API service

builder.Build().Run();
