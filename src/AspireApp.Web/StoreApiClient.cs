using System.Net.Http.Json;

namespace AspireApp.Web;

public interface IStoreApiClient
{
    Task<IEnumerable<CustomerSummary>> GetCustomersAsync();
    Task<IEnumerable<ProductSummary>> GetProductsAsync();
    Task<IEnumerable<OrderSummary>> GetOrdersAsync();
    Task<StoreStats> GetStatsAsync();
}

public class StoreApiClient : IStoreApiClient
{
    private readonly HttpClient _httpClient;

    public StoreApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CustomerSummary>> GetCustomersAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<CustomerSummary>>("/customers") ?? Array.Empty<CustomerSummary>();
    }

    public async Task<IEnumerable<ProductSummary>> GetProductsAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<ProductSummary>>("/products") ?? Array.Empty<ProductSummary>();
    }

    public async Task<IEnumerable<OrderSummary>> GetOrdersAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<OrderSummary>>("/orders") ?? Array.Empty<OrderSummary>();
    }

    public async Task<StoreStats> GetStatsAsync()
    {
        return await _httpClient.GetFromJsonAsync<StoreStats>("/stats") ?? new StoreStats(0, 0, 0, 0, 0);
    }
}

public record CustomerSummary(int Id, string FirstName, string LastName, string Email, string? City);
public record ProductSummary(int Id, string Name, decimal Price, string Category, int StockQuantity);
public record OrderSummary(int Id, string OrderNumber, DateTime OrderDate, string Status, decimal TotalAmount, string CustomerName);
public record StoreStats(int CustomerCount, int ProductCount, int OrderCount, int OrderItemCount, decimal TotalRevenue); 