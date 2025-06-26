# Payments.Services

## Overview

The **Services** project contains the implementation of business logic and orchestrates operations between the Domain and Infrastructure layers. It implements the interfaces defined in **Payments.Services.Abstractions** and contains the core business operations.

## Responsibilities

- **Business Logic Implementation**: Contains the actual business rules and operations
- **Orchestration**: Coordinates between repositories, entities, and DTOs
- **Data Mapping**: Converts between domain entities and DTOs
- **Transaction Management**: Works with Unit of Work pattern
- **Validation**: Enforces business rules and constraints

## Project Structure

```
Payments.Services/
├── ServiceManager.cs    # Facade implementation for all services
├── BillerService.cs     # Biller business logic implementation
├── CustomerService.cs   # Customer business logic implementation
└── InvoiceService.cs    # Invoice business logic implementation
```

## Service Implementations

### ServiceManager
Facade implementation that provides access to all services:

```csharp
public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IBillerService> _lazyBillerService;
    private readonly Lazy<ICustomerService> _lazyCustomerService;
    private readonly Lazy<IInvoiceService> _lazyInvoiceService;

    public ServiceManager(IRepositoryManager repositoryManager)
    {
        _lazyBillerService = new Lazy<IBillerService>(() => new BillerService(repositoryManager));
        _lazyCustomerService = new Lazy<ICustomerService>(() => new CustomerService(repositoryManager));
        _lazyInvoiceService = new Lazy<IInvoiceService>(() => new InvoiceService(repositoryManager));
    }

    public IBillerService BillerService => _lazyBillerService.Value;
    public ICustomerService CustomerService => _lazyCustomerService.Value;
    public IInvoiceService InvoiceService => _lazyInvoiceService.Value;
}
```

**Features:**
- **Lazy Loading**: Services are created only when first accessed
- **Dependency Injection**: Accepts repository manager for data access
- **Single Responsibility**: Manages service instances

### BillerService
Implements business logic for Biller operations:

```csharp
internal sealed class BillerService : IBillerService
{
    private readonly IRepositoryManager _repositoryManager;

    public BillerService(IRepositoryManager repositoryManager) 
        => _repositoryManager = repositoryManager;
}
```

**Key Operations:**

1. **GetAllAsync()**: Retrieve all billers
2. **GetByIdAsync()**: Get specific biller with validation
3. **CreateAsync()**: Create new biller with business rules
4. **UpdateAsync()**: Update existing biller with validation
5. **DeleteAsync()**: Delete biller with cleanup operations

### CustomerService
Implements business logic for Customer operations:

```csharp
internal sealed class CustomerService : ICustomerService
{
    private readonly IRepositoryManager _repositoryManager;

    public CustomerService(IRepositoryManager repositoryManager) 
        => _repositoryManager = repositoryManager;
}
```

**Key Operations:**

1. **GetAllByBillerIdAsync()**: Get customers for specific biller
2. **GetByIdAsync()**: Get customer with biller validation
3. **CreateAsync()**: Create customer for valid biller
4. **UpdateAsync()**: Update customer with validation
5. **DeleteAsync()**: Delete customer with ownership validation

### InvoiceService
Implements business logic for Invoice operations:

```csharp
internal sealed class InvoiceService : IInvoiceService
{
    private readonly IRepositoryManager _repositoryManager;

    public InvoiceService(IRepositoryManager repositoryManager) 
        => _repositoryManager = repositoryManager;
}
```

**Key Operations:**

1. **GetAllByBillerIdAsync()**: Get invoices for specific biller
2. **GetAllByCustomerIdAsync()**: Get invoices for specific customer
3. **GetByIdAsync()**: Get invoice with validation
4. **CreateAsync()**: Create invoice with business rules
5. **UpdateAsync()**: Update invoice with validation

## Business Logic Examples

### Biller Validation
```csharp
public async Task<BillerDto> GetByIdAsync(Guid billerId, CancellationToken cancellationToken = default)
{
    var biller = await _repositoryManager.BillerRepository.GetByIdAsync(billerId, cancellationToken);

    if (biller is null)
    {
        throw new BillerNotFoundException(billerId);
    }

    var billerDto = biller.Adapt<BillerDto>();
    return billerDto;
}
```

### Customer Creation with Biller Validation
```csharp
public async Task<CustomerDto> CreateAsync(Guid billerId, CustomerForCreationDto customerForCreationDto, CancellationToken cancellationToken = default)
{
    var biller = await _repositoryManager.BillerRepository.GetByIdAsync(billerId, cancellationToken);
    if (biller is null)
    {
        throw new BillerNotFoundException(billerId);
    }

    var customer = customerForCreationDto.Adapt<Customer>();
    customer.BillerId = biller.Id;
    customer.PublicId = Guid.NewGuid();
    customer.CreatedAt = DateTime.UtcNow;
    customer.UpdatedAt = DateTime.UtcNow;

    _repositoryManager.BillerRepository.Insert(customer);
    await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

    var customerDto = customer.Adapt<CustomerDto>();
    return customerDto;
}
```

## Key Features

### 1. **Data Mapping**
Uses Mapster library for efficient object mapping:
```csharp
var billerDto = biller.Adapt<BillerDto>();
var biller = billerForCreationDto.Adapt<Biller>();
```

### 2. **Exception Handling**
Throws domain-specific exceptions for business rule violations:
- `BillerNotFoundException`
- `CustomerNotFoundException`
- `InvoiceNotFoundException`

### 3. **Transaction Management**
Works with Unit of Work pattern for data consistency:
```csharp
await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
```

### 4. **Async Operations**
All operations are asynchronous for better performance and scalability.

### 5. **Cancellation Support**
Supports cancellation tokens for proper resource management.

## Dependencies

- **Payments.Domain**: Uses entities, repositories, and exceptions
- **Payments.Services.Abstractions**: Implements service interfaces
- **Payments.Contracts**: Works with DTOs for input/output
- **Mapster**: For object mapping between entities and DTOs

## Design Patterns

### 1. **Facade Pattern (ServiceManager)**
Simplifies access to multiple services through a single interface.

### 2. **Repository Pattern**
Uses repository interfaces for data access without knowing implementation details.

### 3. **Unit of Work Pattern**
Coordinates transactions across multiple repositories.

### 4. **Lazy Loading**
Services are instantiated only when needed for better performance.

## Benefits

1. **Business Logic Centralization**: All business rules are in one place
2. **Testability**: Easy to unit test business logic in isolation
3. **Maintainability**: Clear separation of concerns
4. **Reusability**: Business logic can be reused across different presentation layers
5. **Flexibility**: Can change business rules without affecting other layers

## Best Practices

1. **Keep Services Focused**: Each service should handle one domain area
2. **Use Domain Exceptions**: Throw domain-specific exceptions for business rule violations
3. **Validate Input**: Always validate input data before processing
4. **Handle Transactions**: Use Unit of Work for data consistency
5. **Async Operations**: Use async/await for better performance

## Testing

Services can be easily unit tested by mocking the repository manager and verifying business logic in isolation.