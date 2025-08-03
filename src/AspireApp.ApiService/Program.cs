using Database.EfCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddProblemDetails();

// Add DbContext
builder.Services.AddDbContext<StoreDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();

// Add endpoints
app.MapGet("/customers", async (StoreDbContext context) =>
{
    var customers = await context.Customers
        .Take(10)
        .Select(c => new { c.Id, c.FirstName, c.LastName, c.Email, c.City })
        .ToListAsync();
    return Results.Ok(customers);
});

app.MapGet("/products", async (StoreDbContext context) =>
{
    var products = await context.Products
        .Where(p => p.IsActive)
        .Take(10)
        .Select(p => new { p.Id, p.Name, p.Price, p.Category, p.StockQuantity })
        .ToListAsync();
    return Results.Ok(products);
});

app.MapGet("/orders", async (StoreDbContext context) =>
{
    var orders = await context.Orders
        .Include(o => o.Customer)
        .Take(10)
        .Select(o => new { 
            o.Id, 
            o.OrderNumber, 
            o.OrderDate, 
            o.Status, 
            o.TotalAmount,
            CustomerName = $"{o.Customer.FirstName} {o.Customer.LastName}"
        })
        .ToListAsync();
    return Results.Ok(orders);
});

app.MapGet("/stats", async (StoreDbContext context) =>
{
    var stats = new
    {
        CustomerCount = await context.Customers.CountAsync(),
        ProductCount = await context.Products.CountAsync(),
        OrderCount = await context.Orders.CountAsync(),
        OrderItemCount = await context.OrderItems.CountAsync(),
        TotalRevenue = await context.Orders.SumAsync(o => o.TotalAmount)
    };
    return Results.Ok(stats);
});

app.Run();
