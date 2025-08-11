using AspireApp.Web;
using AspireApp.Web.Components;
using Database.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add DbContext
builder.Services.AddDbContext<StoreDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("store") 
        ?? builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Add API services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Store API", 
        Version = "v1",
        Description = "API endpoints for the Store Management System"
    });
});

// Add CORS for API access
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register StoreApiClient as a local service (no HTTP needed)
builder.Services.AddScoped<IStoreApiClient, LocalStoreApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    // Enable Swagger in development
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseStaticFiles();
app.UseAntiforgery();

// Map API endpoints
var apiGroup = app.MapGroup("/api");

apiGroup.MapGet("/customers", async (StoreDbContext context) =>
{
    var customers = await context.Customers
        .Take(10)
        .Select(c => new { c.Id, c.FirstName, c.LastName, c.Email, c.City })
        .ToListAsync();
    return Results.Ok(customers);
})
.WithName("GetCustomers")
.WithOpenApi()
.Produces(200);

apiGroup.MapGet("/products", async (StoreDbContext context) =>
{
    var products = await context.Products
        .Where(p => p.IsActive)
        .Take(10)
        .Select(p => new { p.Id, p.Name, p.Price, p.Category, p.StockQuantity })
        .ToListAsync();
    return Results.Ok(products);
})
.WithName("GetProducts")
.WithOpenApi()
.Produces(200);

apiGroup.MapGet("/orders", async (StoreDbContext context) =>
{
    var orders = await context.Orders
        .Include(o => o.Customer)
        .Take(10)
        .Select(o => new 
        { 
            o.Id, 
            o.OrderNumber, 
            o.OrderDate, 
            o.Status, 
            o.TotalAmount,
            CustomerName = $"{o.Customer.FirstName} {o.Customer.LastName}"
        })
        .ToListAsync();
    return Results.Ok(orders);
})
.WithName("GetOrders")
.WithOpenApi()
.Produces(200);

apiGroup.MapGet("/stats", async (StoreDbContext context) =>
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
})
.WithName("GetStats")
.WithOpenApi()
.Produces(200);

// Map Razor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();