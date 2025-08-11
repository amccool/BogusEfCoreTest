# Bogus EF Core Test - Store Management System

This solution demonstrates a comprehensive .NET Aspire application that uses Entity Framework Core with Bogus for generating realistic test data. The system implements a complete store management application with customers, products, orders, and order items.

## Architecture Overview

The solution follows a consolidated architecture using .NET Aspire for orchestration:

```
┌─────────────────────────────────────┐
│       AspireApp.Web                 │
│  (Blazor UI + API + Swagger)        │
│                                     │
│  • Blazor Server Pages              │
│  • REST API Endpoints               │
│  • Swagger Documentation            │
│  • Direct Database Access           │
└──────────────┬──────────────────────┘
               │
               ▼
┌──────────────────────────────────────┐
│         SQL Server                   │
│  (Container managed by Aspire)       │
│                                      │
│  • SSDT Schema Deployment            │
│  • Bogus Data Seeding                │
└──────────────────────────────────────┘
```

## Project Structure

```
BogusEfCoreTest/
├── BogusEfCoreTest.sln               # Main solution file
├── README.md                          # This file
├── TODO_TOMORROW.md                   # Development notes
├── user_prompts.md                    # Session history
└── src/
    ├── AspireApp.AppHost/            # Aspire orchestration
    ├── AspireApp.Web/                # Consolidated Web App
    │   ├── Components/
    │   │   └── Pages/                # Blazor pages
    │   │       ├── Customers.razor   # Customer management
    │   │       ├── Products.razor    # Product catalog
    │   │       ├── Orders.razor      # Order tracking
    │   │       └── StoreStats.razor  # Statistics dashboard
    │   ├── LocalStoreApiClient.cs    # Direct DB access service
    │   └── Program.cs                 # API endpoints + Swagger
    ├── AspireApp.ServiceDefaults/    # Shared Aspire defaults
    ├── Database/
    │   ├── Database.EfCore/          # EF Core entities & context
    │   │   └── Entities/
    │   │       ├── Customer.cs
    │   │       ├── Order.cs
    │   │       ├── OrderItem.cs
    │   │       └── Product.cs
    │   └── Database.SqlProj/         # SSDT database schema
    │       └── Schema/
    │           ├── Customers.sql
    │           ├── Orders.sql
    │           ├── OrderItems.sql
    │           └── Products.sql
    └── SeedData/
        └── SeedData.Generator/       # Bogus data seeding service
            ├── DatabaseSeeder.cs     # Fixed to use navigation properties
            └── BogusOperations.cs
```

## Key Features

### 1. Consolidated Web Application
- **Blazor Server UI**: Interactive web interface for data management
- **REST API Endpoints**: Exposed at `/api/*` for external access
- **Swagger Documentation**: Available at `/swagger` for API exploration
- **Direct Database Access**: No inter-service HTTP calls for better performance

### 2. .NET Aspire Orchestration
- **SQL Server Container**: Automatically creates and manages SQL Server
- **SSDT Deployment**: Database schema deployed via SqlProject
- **Service Coordination**: Manages startup order and dependencies
- **Configuration Management**: Handles connection strings automatically

### 3. Database Schema (SSDT)
- **Customers Table**: Identity column for ID, customer information with addresses
- **Products Table**: Product catalog with categories and inventory
- **Orders Table**: Order management with customer foreign key
- **OrderItems Table**: Order line items with product references
- **Proper Indexing**: Optimized for common query patterns
- **Foreign Key Constraints**: Maintains data integrity

### 4. Entity Framework Core
- **Navigation Properties**: Full relationship mapping (fixed for identity columns)
- **LocalStoreApiClient**: Direct database access without HTTP overhead
- **Configuration**: Fluent API for complex mappings
- **SQL Server Identity**: Properly handles auto-generated IDs

### 5. Bogus Data Seeding (Fixed)
- **Navigation Property Usage**: Sets Customer object instead of CustomerId
- **Identity Column Support**: EF Core handles ID generation
- **Realistic Data**: Generates 100 customers, 50 products, 200 orders
- **Relationship Management**: Maintains referential integrity

## Database Schema

### Customers (Identity Column)
```sql
[Id] INT IDENTITY(1,1) NOT NULL  -- Auto-generated
```
- Personal information (name, email, phone)
- Address details (street, city, state, zip, country)
- Audit fields (created/modified dates)

### Products (Identity Column)
```sql
[Id] INT IDENTITY(1,1) NOT NULL  -- Auto-generated
```
- Product details (name, description, price)
- Inventory management (SKU, stock quantity)
- Categorization and status

### Orders (Identity Column)
```sql
[Id] INT IDENTITY(1,1) NOT NULL  -- Auto-generated
[CustomerId] INT NOT NULL         -- Foreign key to Customers
```
- Order tracking (order number, status, dates)
- Customer relationship via navigation property
- Financial information (total amount)

### OrderItems (Identity Column)
```sql
[Id] INT IDENTITY(1,1) NOT NULL  -- Auto-generated
[OrderId] INT NOT NULL            -- Foreign key to Orders
[ProductId] INT NOT NULL          -- Foreign key to Products
```
- Order line items with quantities
- Pricing information (unit price, total price)

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Docker Desktop (for SQL Server container)
- Visual Studio 2022 or VS Code
- .NET Aspire workload

### Running the Application

1. **Clone and Build**
   ```bash
   git clone <repository-url>
   cd BogusEfCoreTest
   dotnet build
   ```

2. **Run with Aspire**
   ```bash
   cd src/AspireApp.AppHost
   dotnet run
   ```

3. **Access the Application**
   - Aspire Dashboard: https://localhost:17015 (check console for token)
   - Web Application: Click "webfrontend" in Aspire dashboard
   - Swagger API Docs: `[webfrontend-url]/swagger`

### Application Pages

- **Dashboard** (`/`): Overview with statistics and recent data
- **Customers** (`/customers`): View customer list
- **Products** (`/products`): Browse product catalog with stock status
- **Orders** (`/orders`): Track orders with status indicators
- **Statistics** (`/store-stats`): Detailed store metrics

### API Endpoints

All endpoints are prefixed with `/api`:

- `GET /api/customers` - List top 10 customers
- `GET /api/products` - List top 10 active products  
- `GET /api/orders` - List top 10 orders with customer names
- `GET /api/stats` - Get store statistics (counts and revenue)

## Data Seeding Solution

### The Identity Column Problem (Solved)
The original issue: Bogus was generating Customer entities with Id = 0, causing foreign key violations when creating Orders due to SQL Server IDENTITY columns.

### The Fix: Navigation Properties
```csharp
// OLD (broken) - tried to use CustomerId directly
.RuleFor(x => x.CustomerId, f => f.PickRandom(customers).Id) // Id was 0!

// NEW (working) - use navigation property
.RuleFor(x => x.Customer, f => f.PickRandom(customerList))
```

EF Core's change tracker handles the identity resolution automatically when using navigation properties.

## Configuration

### Connection Strings
Aspire automatically injects the connection string for the SQL Server database. The Web app uses:
```csharp
builder.Configuration.GetConnectionString("store")
```

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- Database connection strings are injected by Aspire

## Development Workflow

### Adding New Features
1. Update SSDT schema in `Database.SqlProj/Schema/`
2. Add/modify EF Core entities in `Database.EfCore/Entities/`
3. Update `StoreDbContext` if needed
4. Modify `DatabaseSeeder` for test data generation
5. Add API endpoints in `Program.cs`
6. Create Blazor pages in `Components/Pages/`

### Testing the Seeder
1. Run the Aspire application
2. Check the Aspire dashboard for service status
3. Verify data in the web UI or via API endpoints
4. Use Swagger UI to test API endpoints directly

## Best Practices Implemented

### Database Design
- **Identity Columns**: Auto-generated primary keys
- **Foreign Keys**: Enforced referential integrity
- **Indexing**: Optimized for common queries
- **Audit Fields**: Created/modified timestamps

### EF Core Usage
- **Navigation Properties**: Proper relationship management
- **Direct DB Access**: LocalStoreApiClient for Blazor components
- **Fluent API**: Complex configuration in DbContext
- **Identity Resolution**: Let EF Core handle ID generation

### Bogus Integration
- **Navigation Properties**: Use object references, not IDs
- **Deterministic Seeds**: Consistent data across runs
- **Realistic Data**: Comprehensive test scenarios
- **Bulk Operations**: Efficient SaveChangesAsync calls

### Architecture Benefits
- **Simplified**: Single web app with all functionality
- **Performance**: No inter-service HTTP calls
- **Maintainability**: All web code in one project
- **Documentation**: Swagger UI for API exploration

## Troubleshooting

### Common Issues

1. **Customer ID = 0 Error**
   - Solution: Use navigation properties in Bogus
   - EF Core handles identity column values

2. **Database Connection**
   - Check SQL Server container is running in Docker
   - Verify connection string in Aspire dashboard

3. **Data Not Showing**
   - Ensure seeder completed successfully
   - Check Aspire dashboard for service status
   - Verify database has data using API endpoints

### Debugging
- Use Aspire dashboard for service monitoring
- Check application logs in console output
- Test API endpoints via Swagger UI
- Verify data using Blazor pages

## Recent Changes

### Consolidation (Latest)
- Removed separate ApiService project
- Moved all API endpoints to Web project
- Added Swagger/OpenAPI documentation
- Implemented LocalStoreApiClient for direct DB access
- Created all Blazor pages for data viewing

### Bug Fixes
- Fixed Bogus seeder to use navigation properties
- Resolved identity column foreign key issues
- Corrected Order-Customer relationship

## Future Enhancements

- **Authentication**: Add user management and security
- **Pagination**: Implement paging for large datasets
- **Search/Filter**: Add search capabilities to pages
- **Export**: CSV/Excel export functionality
- **Real-time Updates**: SignalR for live data updates
- **Unit Tests**: Add comprehensive test coverage
- **CI/CD**: Automated deployment pipeline

## License

This project is provided as-is for educational and demonstration purposes.