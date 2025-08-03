using Database.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SeedData.Generator;

var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);

// Add configuration
//builder.Configuration.AddJsonFile("appsettings.json", optional: true);
//builder.Configuration.AddEnvironmentVariables();

// Add DbContext
builder.Services.AddDbContext<StoreDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("store");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'store' not found.");
    }
    options.UseSqlServer(connectionString);
});

// Add Database Seeder
builder.Services.AddSingleton<DatabaseSeeder>();

// Add background service for seeding
builder.Services.AddHostedService<DatabaseSeedingService>();

var host = builder.Build();
await host.RunAsync(); 