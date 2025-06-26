# Payments.Web

## Overview

The **Web** project is the main ASP.NET Core application that serves as the entry point and orchestrates all the layers of the Onion Architecture. It contains the application configuration, dependency injection setup, middleware pipeline, and serves as the host for the entire application.

## Responsibilities

- **Application Host**: Main entry point for the ASP.NET Core application
- **Dependency Injection**: Configure and register all services
- **Middleware Pipeline**: Set up HTTP request/response pipeline
- **Configuration Management**: Handle application settings and environment-specific configuration
- **Database Initialization**: Apply Entity Framework migrations on startup
- **Exception Handling**: Global exception handling middleware
- **API Documentation**: Swagger/OpenAPI configuration

## Project Structure

```
Payments.Web/
├── Program.cs                      # Application entry point
├── Startup.cs                      # Application configuration
├── appsettings.json               # Application settings
├── appsettings.Development.json   # Development-specific settings
├── Middleware/                     # Custom middleware
│   └── ExceptionHandlingMiddleware.cs
├── Properties/                     # Project properties
│   └── launchSettings.json
└── Dockerfile                     # Docker container configuration
```

## Core Components

### Program.cs
The application entry point that configures the host and applies database migrations:

```csharp
public class Program
{
    public static async Task Main(string[] args)
    {
        var webHost = CreateHostBuilder(args).Build();
        await ApplyMigrations(webHost.Services);
        await webHost.RunAsync();
    }

    private static async Task ApplyMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        await using RepositoryDbContext dbContext = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}
```

**Key Features:**
- **Automatic Migrations**: Applies EF Core migrations on startup
- **Async Startup**: Proper async initialization
- **Host Configuration**: Sets up the ASP.NET Core host

### Startup.cs
Configures the application services and middleware pipeline:

```csharp
public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Controller configuration
        services.AddControllers()
            .AddApplicationPart(typeof(Payments.Presentation.AssemblyReference).Assembly);

        // Swagger configuration
        services.AddSwaggerGen(c =>
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payments API", Version = "v1" }));

        // Service registration
        services.AddScoped<IServiceManager, ServiceManager>();
        services.AddScoped<IRepositoryManager, RepositoryManager>();

        // Database configuration
        services.AddDbContextPool<RepositoryDbContext>(builder =>
        {
            var connectionString = Configuration.GetConnectionString("Database");
            builder.UseNpgsql(connectionString);
        });

        // Middleware registration
        services.AddTransient<ExceptionHandlingMiddleware>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payments API v1"));
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
```

## Configuration

### appsettings.json
Main application configuration:

```json
{
  "ConnectionStrings": {
    "Database": "Host=payments.db;Database=payments;Username=postgres;Password=postgres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json
Development-specific settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

## Middleware

### ExceptionHandlingMiddleware
Global exception handling that converts domain exceptions to appropriate HTTP responses:

```csharp
internal sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new { error = exception.Message };
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

## Dependency Injection

### Service Registration
All services are registered in the DI container:

```csharp
// Business services
services.AddScoped<IServiceManager, ServiceManager>();

// Repository services
services.AddScoped<IRepositoryManager, RepositoryManager>();

// Database context
services.AddDbContextPool<RepositoryDbContext>(builder =>
{
    var connectionString = Configuration.GetConnectionString("Database");
    builder.UseNpgsql(connectionString);
});

// Middleware
services.AddTransient<ExceptionHandlingMiddleware>();
```

### Controller Registration
Controllers from the Presentation layer are registered:

```csharp
services.AddControllers()
    .AddApplicationPart(typeof(Payments.Presentation.AssemblyReference).Assembly);
```

## API Documentation

### Swagger Configuration
Swagger is configured for API documentation:

```csharp
services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Payments API", 
        Version = "v1",
        Description = "A RESTful API for managing billers, customers, and invoices"
    }));
```

### Available Endpoints
The API provides the following endpoints:

- **Billers**: `/api/billers` - CRUD operations for billers
- **Customers**: `/api/billers/{billerId}/customers` - CRUD operations for customers
- **Invoices**: `/api/billers/{billerId}/invoices` - CRUD operations for invoices

## Docker Support

### Dockerfile
The project includes a Dockerfile for containerization:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Backend/Web/Web.csproj", "Backend/Web/"]
COPY ["Backend/Presentation/Presentation.csproj", "Backend/Presentation/"]
COPY ["Backend/Services/Services.csproj", "Backend/Services/"]
COPY ["Backend/Services.Abstractions/Services.Abstractions.csproj", "Backend/Services.Abstractions/"]
COPY ["Backend/Domain/Domain.csproj", "Backend/Domain/"]
COPY ["Backend/Persistence/Persistence.csproj", "Backend/Persistence/"]
COPY ["Backend/Contracts/Contracts.csproj", "Backend/Contracts/"]
RUN dotnet restore "Backend/Web/Web.csproj"
COPY . .
WORKDIR "/src/Backend/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]
```

### Docker Compose
The application can be run using Docker Compose:

```yaml
version: '3.8'
services:
  payments-api:
    build:
      context: .
      dockerfile: Backend/Web/Dockerfile
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    depends_on:
      - payments-db

  payments-db:
    image: postgres:15
    environment:
      POSTGRES_DB: payments
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - payments-data:/var/lib/postgresql/data

volumes:
  payments-data:
```

## Key Features

### 1. **Automatic Database Migration**
Database schema is automatically created and updated on startup.

### 2. **Global Exception Handling**
All exceptions are caught and converted to appropriate HTTP responses.

### 3. **Swagger Documentation**
Interactive API documentation is available at `/swagger`.

### 4. **Docker Support**
Full containerization support for easy deployment.

### 5. **Environment-Specific Configuration**
Different settings for development and production environments.

## Benefits

1. **Easy Deployment**: Docker support makes deployment straightforward
2. **API Documentation**: Swagger provides interactive documentation
3. **Error Handling**: Global exception handling ensures consistent error responses
4. **Configuration Management**: Environment-specific settings
5. **Database Management**: Automatic migrations ensure database is always up-to-date