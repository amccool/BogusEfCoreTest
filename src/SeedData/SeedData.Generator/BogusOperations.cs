using Database.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SeedData.Generator;

public static class BogusOperations
{
    public static async Task<(bool success, Exception? exception)> CreateDatabaseAndPopulateAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            
            // Following the article's pattern: create/recreate database
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // Create seeder instance
            var seeder = new DatabaseSeeder();

            // Seed the database using the existing seeder
            await seeder.SeedDatabaseAsync(context);

            return (true, null);
        }
        catch (Exception localException)
        {
            return (false, localException);
        }
    }

    public static async Task<(bool success, Exception? exception)> PopulateDatabaseIfEmptyAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            
            // Following the article's pattern: check if tables are populated first
            if (await DataProtection.TablesArePopulatedAsync(serviceProvider))
            {
                // Database already has data, skip seeding (following article's protection pattern)
                return (true, null);
            }

            // Check if database exists and can connect
            var (canConnect, connectException) = await DataProtection.CanConnectAsync(serviceProvider);
            if (!canConnect)
            {
                await context.Database.EnsureCreatedAsync();
            }

            // Create seeder instance and populate
            var seeder = new DatabaseSeeder();
            await seeder.SeedDatabaseAsync(context);

            return (true, null);
        }
        catch (Exception localException)
        {
            return (false, localException);
        }
    }
} 