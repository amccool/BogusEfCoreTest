# Bogus EF Core Test - Store Management System

This solution demonstrates a comprehensive .NET Aspire application that uses Entity Framework Core with Bogus for generating realistic test data. The system implements a complete store management application with customers, products, orders, and order items.

## Architecture Overview

The solution follows a modern microservices architecture using .NET Aspire for orchestration:

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Web Frontend  │    │   API Service   │    │  Database       │
│   (Blazor)      │◄──►│   (Minimal API) │◄──►│  (SQL Server)   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │
                                ▼
                       ┌─────────────────┐
                       │ Database Seeder │
                       │ (Bogus + EF)    │
                       └─────────────────┘
```

## Project Structure

```
bogusefcoretest/
├── BogusEfCoreTest.sln          # Main solution file
└── src/
    ├── AspireApp.AppHost/       # Aspire orchestration
    ├── AspireApp.ApiService/    # REST API service
    ├── AspireApp.Web/           # Blazor Web frontend
    ├── Database/
    │   ├── Database.EfCore/     # EF Core entities & context
    │   └── Database.SqlProj/    # SSDT database schema
    └── SeedData/
        └── SeedData.Generator/  # Bogus data seeding service
```

## Key Features

### 1. .NET Aspire Orchestration
- **SQL Server Resource**: Automatically creates and manages SQL Server database
- **Service Coordination**: Manages startup order and dependencies
- **Configuration Management**: Handles connection strings and environment variables

### 2. SSDT Database Schema
- **Customers Table**: Complete customer information with addresses
- **Products Table**: Product catalog with categories and inventory
- **Orders Table**: Order management with status tracking
- **OrderItems Table**: Order line items with pricing
- **Proper Indexing**: Optimized for common query patterns
- **Foreign Key Constraints**: Maintains data integrity

### 3. Entity Framework Core
- **Code-First Approach**: Entities match SSDT schema exactly
- **Navigation Properties**: Full relationship mapping
- **Configuration**: Fluent API for complex mappings
- **Migration Support**: Ready for schema evolution

### 4. Bogus Data Seeding
- **Deterministic Generation**: Uses local seeds for consistent data
- **Realistic Data**: Generates realistic names, addresses, products
- **Relationship Management**: Maintains referential integrity
- **Scalable**: Can generate thousands of records efficiently

## Database Schema

### Customers
- Personal information (name, email, phone)
- Address details (street, city, state, zip, country)
- Audit fields (created/modified dates)

### Products
- Product details (name, description, price)
- Inventory management (SKU, stock quantity)
- Categorization and status

### Orders
- Order tracking (order number, status, dates)
- Customer relationship
- Financial information (total amount)
- Shipping/billing addresses

### OrderItems
- Order line items with quantities
- Pricing information (unit price, total price)
- Product and order relationships

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (local or container)
- Visual Studio 2022 or VS Code

### Running the Application

1. **Clone and Build**
   ```bash
   git clone <repository-url>
   cd bogusefcoretest
   dotnet build
   ```

2. **Run with Aspire**
   ```bash
   cd src/AspireApp.AppHost
   dotnet run
   ```

3. **Access the Application**
   - Web Dashboard: http://localhost:5000
   - API Endpoints: http://localhost:5001
   - Aspire Dashboard: http://localhost:5002

### API Endpoints

- `GET /customers` - List customers
- `GET /products` - List products
- `GET /orders` - List orders with customer details
- `GET /stats` - Get store statistics

## Data Seeding with Bogus

The solution implements advanced Bogus patterns following best practices:

### Deterministic Generation
```csharp
// Uses local seeds for consistent data across runs
var recordRow = faker.UseSeed(rowId).Generate();
```

### Realistic Data Generation
```csharp
var customerFaker = new Faker<Customer>()
    .RuleFor(x => x.FirstName, f => f.Name.FirstName())
    .RuleFor(x => x.LastName, f => f.Name.LastName())
    .RuleFor(x => x.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
    .RuleFor(x => x.Address, f => f.Address.StreetAddress());
```

### Relationship Management
```csharp
// Maintains referential integrity
.RuleFor(x => x.CustomerId, f => f.PickRandom(customers).Id)
```

## Configuration

### Connection Strings
Aspire automatically manages connection strings for:
- SQL Server database
- Service-to-service communication

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- Database connection strings are injected by Aspire

## Development Workflow

### Adding New Entities
1. Create SSDT schema in `Database.SqlProj`
2. Add EF Core entity in `Database.EfCore`
3. Update `StoreDbContext`
4. Add Bogus generation in `DatabaseSeeder`
5. Update API endpoints
6. Update web frontend

### Schema Changes
1. Modify SSDT schema files
2. Update EF Core entities
3. Generate new migration: `dotnet ef migrations add MigrationName`
4. Update database: `dotnet ef database update`

## Best Practices Implemented

### Database Design
- **Normalization**: Proper table relationships
- **Indexing**: Optimized for common queries
- **Constraints**: Data integrity enforcement
- **Audit Fields**: Created/modified timestamps

### EF Core Usage
- **Code-First**: Maintainable entity definitions
- **Fluent API**: Complex configuration
- **Navigation Properties**: Rich object graphs
- **Lazy Loading**: Efficient data access

### Bogus Integration
- **Determinism**: Consistent data across runs
- **Local Seeds**: Schema change resilience
- **Realistic Data**: Comprehensive test scenarios
- **Performance**: Efficient bulk generation

### Aspire Benefits
- **Service Discovery**: Automatic service location
- **Configuration**: Centralized settings management
- **Monitoring**: Built-in observability
- **Development Experience**: Simplified local development

## Troubleshooting

### Common Issues

1. **Database Connection**
   - Ensure SQL Server is running
   - Check connection strings in Aspire dashboard

2. **Data Seeding**
   - Verify Bogus package versions
   - Check for constraint violations

3. **Service Communication**
   - Verify service discovery in Aspire
   - Check network connectivity

### Debugging
- Use Aspire dashboard for service monitoring
- Check application logs in each service
- Verify database state with SQL Server Management Studio

## References

This implementation follows patterns from:
- [Taking EF Core data seeding to the next level with Bogus](https://stenbrinke.nl/blog/taking-ef-core-data-seeding-to-the-next-level-with-bogus/)
- [Using EF Core and Bogus](https://dev.to/karenpayneoregon/using-ef-core-and-bogus-246d)
- [Seeding databases conditionally with Faker in .NET Core](https://medium.com/@ashishnimrot/seeding-databases-conditionally-with-faker-in-net-core-d2ff5c11fc71)

## Future Enhancements

- **EF Core Power Tools**: Generate entities from existing database
- **Advanced Queries**: Complex reporting and analytics
- **Caching**: Redis integration for performance
- **Authentication**: User management and security
- **API Documentation**: Swagger/OpenAPI integration
- **Testing**: Unit and integration tests
- **CI/CD**: Automated deployment pipeline

## License

This project is provided as-is for educational and demonstration purposes. 