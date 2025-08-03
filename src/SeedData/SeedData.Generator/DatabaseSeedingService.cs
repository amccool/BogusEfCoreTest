using Database.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SeedData.Generator;

public class DatabaseSeedingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeedingService> _logger;

    public DatabaseSeedingService(IServiceProvider serviceProvider, ILogger<DatabaseSeedingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting database seeding service...");

            // Populate with Bogus data (SSDT schema is already deployed by Community Toolkit)
            var (success, exception) = await BogusOperations.PopulateDatabaseIfEmptyAsync(_serviceProvider);

            if (!success)
            {
                _logger.LogError(exception, "Failed to populate database");
                throw exception ?? new InvalidOperationException("Failed to populate database");
            }

            // Log statistics after successful seeding
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

            var customerCount = await context.Customers.CountAsync(stoppingToken);
            var productCount = await context.Products.CountAsync(stoppingToken);
            var orderCount = await context.Orders.CountAsync(stoppingToken);
            var orderItemCount = await context.OrderItems.CountAsync(stoppingToken);

            _logger.LogInformation("Seed data statistics:");
            _logger.LogInformation("- Customers: {CustomerCount}", customerCount);
            _logger.LogInformation("- Products: {ProductCount}", productCount);
            _logger.LogInformation("- Orders: {OrderCount}", orderCount);
            _logger.LogInformation("- Order Items: {OrderItemCount}", orderItemCount);

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
        finally
        {
            // Stop the application after seeding is complete
            var hostApplicationLifetime = _serviceProvider.GetRequiredService<IHostApplicationLifetime>();
            hostApplicationLifetime.StopApplication();
        }
    }


} 