using Database.EfCore;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.Web;

public class LocalStoreApiClient : IStoreApiClient
{
    private readonly StoreDbContext _context;

    public LocalStoreApiClient(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerSummary>> GetCustomersAsync()
    {
        var customers = await _context.Customers
            .Take(10)
            .Select(c => new CustomerSummary(
                c.Id, 
                c.FirstName, 
                c.LastName, 
                c.Email, 
                c.City))
            .ToListAsync();
        
        return customers;
    }

    public async Task<IEnumerable<ProductSummary>> GetProductsAsync()
    {
        var products = await _context.Products
            .Where(p => p.IsActive)
            .Take(10)
            .Select(p => new ProductSummary(
                p.Id, 
                p.Name, 
                p.Price, 
                p.Category, 
                p.StockQuantity))
            .ToListAsync();
        
        return products;
    }

    public async Task<IEnumerable<OrderSummary>> GetOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Take(10)
            .Select(o => new OrderSummary(
                o.Id, 
                o.OrderNumber, 
                o.OrderDate, 
                o.Status, 
                o.TotalAmount,
                $"{o.Customer.FirstName} {o.Customer.LastName}"))
            .ToListAsync();
        
        return orders;
    }

    public async Task<StoreStats> GetStatsAsync()
    {
        var customerCount = await _context.Customers.CountAsync();
        var productCount = await _context.Products.CountAsync();
        var orderCount = await _context.Orders.CountAsync();
        var orderItemCount = await _context.OrderItems.CountAsync();
        var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
        
        return new StoreStats(
            customerCount, 
            productCount, 
            orderCount, 
            orderItemCount, 
            totalRevenue);
    }
}