using Bogus;
using Database.EfCore;
using Database.EfCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace SeedData.Generator;

public class DatabaseSeeder
{
    public IReadOnlyCollection<Customer> Customers { get; }
    public IReadOnlyCollection<Product> Products { get; }
    public IReadOnlyCollection<Order> Orders { get; }
    public IReadOnlyCollection<OrderItem> OrderItems { get; }

    public DatabaseSeeder()
    {
        // Generate data in the correct order to maintain referential integrity
        Customers = GenerateCustomers(amount: 100);
        Products = GenerateProducts(amount: 50);
        Orders = GenerateOrders(amount: 200, customers: Customers);
        OrderItems = GenerateOrderItems(amount: 500, orders: Orders, products: Products);
    }

    private static IReadOnlyCollection<Customer> GenerateCustomers(int amount)
    {
        //var customerId = 1;
        var customerFaker = new Faker<Customer>()
            //.RuleFor(x => x.Id, f => customerId++)
            .RuleFor(x => x.FirstName, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
            .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.Address, f => f.Address.StreetAddress())
            .RuleFor(x => x.City, f => f.Address.City())
            .RuleFor(x => x.State, f => f.Address.State())
            .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
            .RuleFor(x => x.Country, f => f.Address.Country())
            .RuleFor(x => x.DateOfBirth, f => f.Date.Past(50, DateTime.Now.AddYears(-18)))
            .RuleFor(x => x.CreatedDate, f => f.Date.Past(2, DateTime.Now))
            .RuleFor(x => x.ModifiedDate, (f, c) => c.CreatedDate);

        var customers = Enumerable.Range(1, amount)
            .Select(i => SeedRow(customerFaker, i))
            .ToList();

        return customers;
    }

    private static IReadOnlyCollection<Product> GenerateProducts(int amount)
    {
        //var productId = 1;
        var productFaker = new Faker<Product>()
            //.RuleFor(x => x.Id, f => productId++)
            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
            .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
            .RuleFor(x => x.Price, f => f.Random.Decimal(10.00m, 1000.00m))
            .RuleFor(x => x.Category, f => f.PickRandom(new[] { "Electronics", "Clothing", "Books", "Home & Garden", "Sports", "Toys", "Automotive", "Health & Beauty" }))
            .RuleFor(x => x.SKU, f => f.Commerce.Ean13())
            .RuleFor(x => x.StockQuantity, f => f.Random.Int(0, 100))
            .RuleFor(x => x.IsActive, f => f.Random.Bool(0.9f)) // 90% chance of being active
            .RuleFor(x => x.CreatedDate, f => f.Date.Past(1, DateTime.Now))
            .RuleFor(x => x.ModifiedDate, (f, c) => c.CreatedDate);

        var products = Enumerable.Range(1, amount)
            .Select(i => SeedRow(productFaker, i))
            .ToList();

        return products;
    }

    private static IReadOnlyCollection<Order> GenerateOrders(int amount, IEnumerable<Customer> customers)
    {
        //var orderId = 1;
        var orderFaker = new Faker<Order>()
            //.RuleFor(x => x.Id, f => orderId++)
            .RuleFor(x => x.CustomerId, f => f.PickRandom(customers).Id)
            .RuleFor(x => x.OrderNumber, f => $"ORD-{f.Random.Int(10000, 99999)}-{f.Random.Int(1000, 9999)}")
            .RuleFor(x => x.OrderDate, f => f.Date.Past(1, DateTime.Now))
            .RuleFor(x => x.Status, f => f.PickRandom(new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" }))
            .RuleFor(x => x.TotalAmount, f => f.Random.Decimal(50.00m, 2000.00m))
            .RuleFor(x => x.ShippingAddress, f => f.Address.FullAddress())
            .RuleFor(x => x.BillingAddress, f => f.Address.FullAddress())
            .RuleFor(x => x.Notes, f => f.Random.Bool(0.3f) ? f.Lorem.Sentence() : null) // 30% chance of having notes
            .RuleFor(x => x.CreatedDate, (f, c) => c.OrderDate)
            .RuleFor(x => x.ModifiedDate, (f, c) => c.CreatedDate);

        var orders = Enumerable.Range(1, amount)
            .Select(i => SeedRow(orderFaker, i))
            .ToList();

        return orders;
    }

    private static IReadOnlyCollection<OrderItem> GenerateOrderItems(int amount, IEnumerable<Order> orders, IEnumerable<Product> products)
    {
        //var orderItemId = 1;
        var orderItemFaker = new Faker<OrderItem>()
            //.RuleFor(x => x.Id, f => orderItemId++)
            .RuleFor(x => x.OrderId, f => f.PickRandom(orders).Id)
            .RuleFor(x => x.ProductId, f => f.PickRandom(products).Id)
            .RuleFor(x => x.Quantity, f => f.Random.Int(1, 5))
            .RuleFor(x => x.UnitPrice, f => f.Random.Decimal(10.00m, 500.00m))
            .RuleFor(x => x.TotalPrice, (f, c) => c.UnitPrice * c.Quantity)
            .RuleFor(x => x.CreatedDate, f => f.Date.Past(1, DateTime.Now))
            .RuleFor(x => x.ModifiedDate, (f, c) => c.CreatedDate);

        var orderItems = Enumerable.Range(1, amount)
            .Select(i => SeedRow(orderItemFaker, i))
            .ToList();

        return orderItems;
    }

    private static T SeedRow<T>(Faker<T> faker, int rowId) where T : class
    {
        var recordRow = faker.UseSeed(rowId).Generate();
        return recordRow;
    }

    public async Task SeedDatabaseAsync(StoreDbContext context)
    {
        // Clear existing data
        context.OrderItems.RemoveRange(context.OrderItems);
        context.Orders.RemoveRange(context.Orders);
        context.Products.RemoveRange(context.Products);
        context.Customers.RemoveRange(context.Customers);
        await context.SaveChangesAsync();

        // Add new seed data
        await context.Customers.AddRangeAsync(Customers);
        await context.SaveChangesAsync();

        await context.Products.AddRangeAsync(Products);
        await context.SaveChangesAsync();

        await context.Orders.AddRangeAsync(Orders);
        await context.SaveChangesAsync();

        await context.OrderItems.AddRangeAsync(OrderItems);
        await context.SaveChangesAsync();
    }
} 