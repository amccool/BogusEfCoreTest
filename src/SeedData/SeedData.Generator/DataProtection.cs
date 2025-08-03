using Database.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SeedData.Generator;

public static class DataProtection
{
    public static async Task<bool> TablesArePopulatedAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            
            if (!await context.Database.CanConnectAsync())
            {
                return false;
            }

            // Check if all main tables have data
            var customerCount = await context.Customers.CountAsync();
            var productCount = await context.Products.CountAsync();
            var orderCount = await context.Orders.CountAsync();
            var orderItemCount = await context.OrderItems.CountAsync();

            // Return true if all tables have at least some data
            return customerCount > 0 && productCount > 0 && orderCount > 0 && orderItemCount > 0;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<(bool success, Exception? exception)> CanConnectAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            var result = await context.Database.CanConnectAsync();
            return (result, null);
        }
        catch (Exception e)
        {
            return (false, e);
        }
    }
} 