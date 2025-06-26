# Payments.Services.Abstractions

## Overview

The **Services.Abstractions** project defines the contracts (interfaces) for the Service Layer in the Onion Architecture. These interfaces define the business operations that can be performed without specifying how they are implemented.

## Responsibilities

- **Service Interfaces**: Define contracts for business operations
- **Dependency Inversion**: Enable dependency injection and loose coupling
- **Testability**: Provide abstractions for unit testing
- **Business Logic Contracts**: Specify what business operations are available

## Project Structure

```
Payments.Services.Abstractions/
├── IBillerService.cs      # Biller business operations interface
├── ICustomerService.cs    # Customer business operations interface
├── IInvoiceService.cs     # Invoice business operations interface
└── IServiceManager.cs     # Service facade interface
```

## Service Interfaces

### IBillerService
Defines business operations for Biller management:

```csharp
public interface IBillerService
{
    Task<IEnumerable<BillerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BillerDto> GetByIdAsync(Guid billerId, CancellationToken cancellationToken = default);
    Task<BillerDto> CreateAsync(BillerForCreationDto billerForCreationDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid billerId, BillerForUpdateDto billerForUpdateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid billerId, CancellationToken cancellationToken = default);
}
```

**Operations:**
- `GetAllAsync()`: Retrieve all billers
- `GetByIdAsync()`: Get specific biller by ID
- `CreateAsync()`: Create a new biller
- `UpdateAsync()`: Update existing biller
- `DeleteAsync()`: Delete a biller

### ICustomerService
Defines business operations for Customer management:

```csharp
public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllByBillerIdAsync(Guid billerId, CancellationToken cancellationToken = default);
    Task<CustomerDto> GetByIdAsync(Guid billerId, Guid customerId, CancellationToken cancellationToken);
    Task<CustomerDto> CreateAsync(Guid billerId, CustomerForCreationDto customerForCreationDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid billerId, Guid customerId, CustomerForUpdateDto customerForUpdateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid billerId, Guid customerId, CancellationToken cancellationToken = default);
}
```

**Operations:**
- `GetAllByBillerIdAsync()`: Get all customers for a specific biller
- `GetByIdAsync()`: Get specific customer by ID (with biller validation)
- `CreateAsync()`: Create a new customer for a biller
- `UpdateAsync()`: Update existing customer (with biller validation)
- `DeleteAsync()`: Delete a customer (with biller validation)

### IInvoiceService
Defines business operations for Invoice management:

```csharp
public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllByBillerIdAsync(Guid billerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<InvoiceDto>> GetAllByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<InvoiceDto> GetByIdAsync(Guid billerId, Guid invoiceId, CancellationToken cancellationToken);
    Task<InvoiceDto> CreateAsync(Guid billerId, Guid customerId, InvoiceForCreationDto invoiceForCreationDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid billerId, Guid invoiceId, InvoiceForUpdateDto invoiceForUpdateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid billerId, Guid invoiceId, CancellationToken cancellationToken = default);
}
```

**Operations:**
- `GetAllByBillerIdAsync()`: Get all invoices for a specific biller
- `GetAllByCustomerIdAsync()`: Get all invoices for a specific customer
- `GetByIdAsync()`: Get specific invoice by ID (with biller validation)
- `CreateAsync()`: Create a new invoice for a customer
- `UpdateAsync()`: Update existing invoice (with biller validation)
- `DeleteAsync()`: Delete an invoice (with biller validation)

### IServiceManager
Facade pattern that provides access to all services:

```csharp
public interface IServiceManager
{
    IBillerService BillerService { get; }
    ICustomerService CustomerService { get; }
    IInvoiceService InvoiceService { get; }
}
```

**Benefits:**
- Single point of access to all services
- Simplified dependency injection
- Consistent service management

## Key Principles

### 1. **Dependency Inversion**
Interfaces depend on abstractions (Domain layer) rather than concrete implementations.

### 2. **Single Responsibility**
Each interface focuses on a specific domain area (Billers, Customers, or Invoices).

### 3. **Async Operations**
All operations are asynchronous to support scalability and responsiveness.

### 4. **Cancellation Support**
Operations accept cancellation tokens for proper resource management.

### 5. **DTO-Based Communication**
Services work with DTOs rather than domain entities for input/output.

## Benefits

1. **Testability**: Easy to mock services for unit testing
2. **Flexibility**: Can swap implementations without affecting consumers
3. **Maintainability**: Clear contracts make the codebase easier to maintain
4. **Scalability**: Async operations support better performance
5. **Loose Coupling**: Consumers depend on abstractions, not implementations

## Usage

### In Presentation Layer
Controllers depend on service interfaces:
```csharp
public class BillersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    
    public BillersController(IServiceManager serviceManager) 
        => _serviceManager = serviceManager;
}
```

### In Dependency Injection
Services are registered in the DI container:
```csharp
services.AddScoped<IServiceManager, ServiceManager>();
```

### In Unit Tests
Interfaces can be easily mocked:
```csharp
var mockServiceManager = new Mock<IServiceManager>();
var mockBillerService = new Mock<IBillerService>();
mockServiceManager.Setup(x => x.BillerService).Returns(mockBillerService.Object);
```

## Design Patterns

### 1. **Facade Pattern (IServiceManager)**
Provides a simplified interface to a complex subsystem of services.

### 2. **Repository Pattern Integration**
Services coordinate between repositories and business logic.

### 3. **Unit of Work Pattern**
Services work with the Unit of Work pattern for transaction management.

## Best Practices

1. **Keep Interfaces Focused**: Each interface should have a single responsibility
2. **Use Async/Await**: All operations should be asynchronous
3. **Include Cancellation**: Support cancellation tokens for proper resource management
4. **Documentation**: Provide clear documentation for interface consumers
5. **Consistent Naming**: Use consistent naming conventions across interfaces

## Implementation

The actual implementations of these interfaces are in the **Payments.Services** project, which contains the business logic and coordinates between the Domain and Infrastructure layers. 