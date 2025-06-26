# Payments.Persistence

## Overview

The **Persistence** project represents the **Infrastructure Layer** in the Onion Architecture. It contains all the data access logic, database configurations, and implementations of the repository interfaces defined in the Domain layer.

## Responsibilities

- **Data Access Implementation**: Implements repository interfaces for database operations
- **Entity Framework Configuration**: Database context and entity configurations
- **Database Migrations**: Entity Framework migrations for database schema
- **Unit of Work Implementation**: Transaction management
- **Repository Implementations**: Concrete implementations of domain repository interfaces

## Repository Creation Guidelines

### When to Use Generic Repository
- **Default approach**: Use `IGenericRepository<T>` for standard CRUD operations
- **Simple entities**: Entities that only need basic Create, Read, Update, Delete operations
- **No custom queries**: When no entity-specific query methods are required

### When to Create Specific Repository
**ONLY create a specific repository interface and implementation when you need:**
- **Custom query methods** that are specific to the entity
- **Complex business logic** in data access layer
- **Entity-specific optimizations** or caching strategies
- **Multiple related entity queries** that don't fit the generic pattern

### Examples:
- ✅ **BillerRepository**: Has custom `GetByPublicIdAsync` method - specific repository needed
- ❌ **CustomerAddress**: Only needs standard CRUD - use `IGenericRepository<CustomerAddress>`
- ❌ **Simple entities**: Use generic repository unless custom methods are required

### Implementation Pattern
```csharp
// For entities with custom methods:
public interface IEntityRepository : IGenericRepository<Entity>
{
    Task<Entity> GetByCustomFieldAsync(string field, CancellationToken cancellationToken = default);
}

// For simple entities:
// Use IGenericRepository<Entity> directly in services
```

## Project Structure

```
Payments.Persistence/
├── RepositoryDbContext.cs           # EF Core database context
├── Repositories/                    # Repository implementations
│   ├── RepositoryManager.cs        # Repository facade implementation
│   ├── UnitOfWork.cs               # Unit of Work implementation
│   ├── BillerRepository.cs         # Biller repository implementation
│   └── GenericRepository.cs        # Generic repository implementation
├── Configurations/                  # Entity configurations
│   ├── BillerConfiguration.cs      # Biller entity configuration
│   ├── CustomerConfiguration.cs    # Customer entity configuration
│   └── InvoiceConfiguration.cs     # Invoice entity configuration
└── Migrations/                      # EF Core migrations
    ├── RepositoryDbContextModelSnapshot.cs
    ├── 20250619202701_InitialCreate.cs
    └── 20250619210202_AddRemainingTables.cs
```

## Core Components

### RepositoryDbContext
The main Entity Framework Core database context:

```csharp
public sealed class RepositoryDbContext : DbContext
{
    public RepositoryDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Biller> Billers { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryDbContext).Assembly);
}
```

**Features:**
- **Dependency Injection**: Accepts DbContextOptions for configuration
- **Entity Sets**: Exposes DbSet properties for each entity
- **Auto-Configuration**: Automatically applies entity configurations

### RepositoryManager
Facade implementation that provides access to all repositories:

```csharp
public sealed class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IBillerRepository> _lazyBillerRepository;
    private readonly Lazy<IUnitOfWork> _lazyUnitOfWork;

    public RepositoryManager(RepositoryDbContext dbContext)
    {
        _lazyBillerRepository = new Lazy<IBillerRepository>(() => new BillerRepository(dbContext));
        _lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(dbContext));
    }

    public IBillerRepository BillerRepository => _lazyBillerRepository.Value;
    public IUnitOfWork UnitOfWork => _lazyUnitOfWork.Value;
}
```

### UnitOfWork
Manages database transactions:

```csharp
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly RepositoryDbContext _dbContext;

    public UnitOfWork(RepositoryDbContext dbContext) => _dbContext = dbContext;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
```

## Repository Implementations

### BillerRepository
Implements biller data access operations:

```csharp
internal sealed class BillerRepository : IBillerRepository
{
    private readonly RepositoryDbContext _dbContext;

    public BillerRepository(RepositoryDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<Biller>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Billers.ToListAsync(cancellationToken);

    public async Task<Biller> GetByIdAsync(Guid billerId, CancellationToken cancellationToken = default) =>
        await _dbContext.Billers.FirstOrDefaultAsync(x => x.PublicId == billerId, cancellationToken);

    public void Insert(Biller biller) => _dbContext.Billers.Add(biller);
    public void Remove(Biller biller) => _dbContext.Billers.Remove(biller);
}
```

### GenericRepository
Generic repository implementation for common operations:

```csharp
internal sealed class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly RepositoryDbContext _dbContext;

    public GenericRepository(RepositoryDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Set<T>().ToListAsync(cancellationToken);

    public async Task<T> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default) =>
        await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);

    public void Insert(T entity) => _dbContext.Set<T>().Add(entity);
    public void Remove(T entity) => _dbContext.Set<T>().Remove(entity);
}
```

## Entity Configurations

### BillerConfiguration
Configures the Biller entity for Entity Framework:

```csharp
internal sealed class BillerConfiguration : IEntityTypeConfiguration<Biller>
{
    public void Configure(EntityTypeBuilder<Biller> builder)
    {
        builder.ToTable(nameof(Biller));
        builder.HasKey(biller => biller.Id);
        builder.Property(biller => biller.Id).ValueGeneratedOnAdd();
        builder.Property(biller => biller.PublicId).IsRequired();
        builder.Property(biller => biller.Name).HasMaxLength(100).IsRequired();
        builder.Property(biller => biller.ApiKey).HasMaxLength(255).IsRequired();
        builder.Property(biller => biller.CreatedAt).IsRequired();
        builder.Property(biller => biller.UpdatedAt).IsRequired();

        builder.HasIndex(biller => biller.PublicId).IsUnique();
    }
}
```

### CustomerConfiguration
Configures the Customer entity for Entity Framework:

```csharp
internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable(nameof(Customer));
        builder.HasKey(customer => customer.Id);
        builder.Property(customer => customer.Id).ValueGeneratedOnAdd();
        builder.Property(customer => customer.PublicId).IsRequired();
        builder.Property(customer => customer.Name).HasMaxLength(100).IsRequired();
        builder.Property(customer => customer.AutopayEnabled).IsRequired();
        builder.Property(customer => customer.CreatedAt).IsRequired();
        builder.Property(customer => customer.UpdatedAt).IsRequired();

        builder.HasIndex(customer => customer.PublicId).IsUnique();
        builder.HasOne<Biller>().WithMany().HasForeignKey(customer => customer.BillerId);
    }
}
```

## Database Migrations

The project includes Entity Framework migrations for database schema management:

- **InitialCreate**: Creates the initial database schema with Billers table
- **AddRemainingTables**: Adds remaining tables (Customers, Invoices, Payments, Users)
- **ModelSnapshot**: Maintains the current model state for migration generation

## Key Features

### 1. **Lazy Loading**
Repositories are instantiated only when first accessed for better performance.

### 2. **Eager Loading**
Uses `Include()` to load related entities when needed.

### 3. **Transaction Management**
Unit of Work pattern ensures data consistency across multiple operations.

### 4. **Async Operations**
All database operations are asynchronous for better scalability.

## Dependencies

- **Payments.Domain**: Implements repository interfaces and uses domain entities
- **Entity Framework Core**: For ORM functionality
- **Npgsql.EntityFrameworkCore.PostgreSQL**: PostgreSQL provider

## Design Patterns

### 1. **Repository Pattern**
Abstracts data access logic behind interfaces.

### 2. **Unit of Work Pattern**
Manages transactions across multiple repositories.

### 3. **Facade Pattern (RepositoryManager)**
Provides simplified access to multiple repositories.

### 4. **Lazy Loading**
Instantiates repositories only when needed.

## Benefits

1. **Data Access Abstraction**: Business logic doesn't depend on specific database technology
2. **Testability**: Easy to mock repositories for unit testing
3. **Flexibility**: Can swap database providers without affecting other layers
4. **Performance**: Optimized queries with eager loading
5. **Maintainability**: Clear separation of data access concerns

## Configuration

### Database Connection
Configured in the Web project's `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=payments;Username=postgres;Password=postgres"
  }
}
```

### Dependency Injection
Registered in the Web project's `Startup.cs`:
```csharp
services.AddDbContextPool<RepositoryDbContext>(builder =>
{
    var connectionString = Configuration.GetConnectionString("Database");
    builder.UseNpgsql(connectionString);
});

services.AddScoped<IRepositoryManager, RepositoryManager>();
```

## Best Practices

1. **Use Async Operations**: All database operations should be async
2. **Include Related Data**: Use eager loading for related entities
3. **Transaction Management**: Use Unit of Work for data consistency
4. **Entity Configurations**: Use Fluent API for precise schema control
5. **Repository Pattern**: Abstract data access behind interfaces