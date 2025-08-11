using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server resource
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent);

var storeDb = sqlServer.AddDatabase("store");
                 
// Add SSDT Project deployment using Community Toolkit
var sqlProject = builder.AddSqlProject<Projects.Database_SqlProj>("sqlproject")
   .WithReference(storeDb)
   .WaitFor(storeDb);

// Add Database Seeding Service (waits for SQL project deployment)
var seedData = builder.AddProject<Projects.SeedData_Generator>("databaseseeder")
    .WithReference(storeDb)
    .WaitForCompletion(sqlProject);

// Add Web Frontend with integrated API
builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(storeDb)
    .WaitForCompletion(seedData);

builder.Build().Run();
