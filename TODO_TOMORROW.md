# TODO - Frontend/API Service Integration

## Date Created: 2025-08-10

## Task for Tomorrow:
Fix the wiring between the frontend and API service

### Issues to Address:
1. **Connection Problem**: The web frontend is not properly connecting to the API service
2. **Service Discovery**: Need to ensure proper Aspire service discovery is configured

### Potential Solutions:

#### Option 1: Fix the Service Connection
- Configure proper HTTP client in `Program.cs` of AspireApp.Web
- Ensure service discovery is working through Aspire
- Add proper base URL configuration for the API client
- Check that CORS is properly configured

#### Option 2: Combine into Single Service
- Merge AspireApp.Web and AspireApp.ApiService into one project
- Simplify the architecture by having a single Blazor Server app with direct database access
- Remove the need for HTTP API calls between services
- Simpler deployment and fewer moving parts

### Current Architecture:
```
Aspire AppHost
├── SQL Server Container
├── Database.SqlProj (SSDT deployment)
├── SeedData.Generator (Bogus seeder)
├── AspireApp.ApiService (API endpoints)
└── AspireApp.Web (Blazor frontend)
```

### Files to Review:
- `src/AspireApp.Web/Program.cs` - Check HTTP client registration
- `src/AspireApp.AppHost/Program.cs` - Review service references
- `src/AspireApp.Web/StoreApiClient.cs` - Verify API client implementation

### Notes:
- The API endpoints are working (verified at `/customers`, `/products`, `/orders`, `/stats`)
- The Blazor pages are created and compile successfully
- The issue is likely in the service discovery/HTTP client configuration