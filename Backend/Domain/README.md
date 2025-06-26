# Payments.Domain

## Overview

The **Domain Layer** is the core of the Onion Architecture. It contains the heart of the business logic and is completely isolated from external concerns. This layer has no dependencies on other layers and defines the contracts that other layers must implement.

## Responsibilities

- **Business Entities**: Core domain objects that represent the business concepts
- **Repository Interfaces**: Contracts for data access operations
- **Domain Exceptions**: Business-specific exception types
- **Domain Services**: Business logic that doesn't belong to specific entities

## Project Structure

```
Payments.Domain/
├── Entities/           # Core business entities
│   ├── Biller.cs      # Biller entity
│   ├── Customer.cs    # Customer entity
│   ├── Invoice.cs     # Invoice entity
│   ├── Payment.cs     # Payment entity
│   └── User.cs        # User entity
├── Repositories/       # Repository interfaces
│   ├── IBillerRepository.cs
│   ├── IGenericRepository.cs
│   ├── IRepositoryManager.cs
│   └── IUnitOfWork.cs
└── Exceptions/         # Domain-specific exceptions
    ├── BadRequestException.cs
    ├── NotFoundException.cs
    └── BillerNotFoundException.cs
```

## Key Principles

### 1. **No External Dependencies**
The Domain layer has zero dependencies on external frameworks, databases, or other layers. It only depends on .NET standard libraries.

### 2. **Rich Domain Model**
Entities contain business logic and validation rules. They are not just data containers (anemic domain model).

### 3. **Repository Pattern**
Repository interfaces define the contract for data access without specifying implementation details.

### 4. **Domain Exceptions**
Custom exception types that represent business rule violations and domain-specific errors.

## Entities

### Biller
Represents a billing entity with properties:
- `Id`: Unique identifier
- `PublicId`: Public-facing GUID
- `Name`: Biller's name
- `ApiKey`: API key for authentication
- `CreatedAt`: Creation timestamp
- `UpdatedAt`: Last update timestamp

### Customer
Represents a customer with properties:
- `Id`: Unique identifier
- `PublicId`: Public-facing GUID
- `BillerId`: Reference to the biller
- `PaymentGatewayId`: Reference to payment gateway
- `Name`: Customer's name
- `AutopayEnabled`: Whether autopay is enabled
- `CreatedAt`: Creation timestamp
- `UpdatedAt`: Last update timestamp

### Invoice
Represents a billing invoice with properties:
- `Id`: Unique identifier
- `PublicId`: Public-facing GUID
- `BillerId`: Reference to the biller
- `CustomerId`: Reference to the customer
- `DueDate`: Payment due date
- `Fees`: JSON document containing fees
- `PassThruFees`: Whether fees are passed through
- `SalesTax`: Sales tax amount
- `TotalAmount`: Total invoice amount
- `Currency`: Currency code
- `Status`: Invoice status
- `CreatedAt`: Creation timestamp
- `UpdatedAt`: Last update timestamp

## Repository Interfaces

### IBillerRepository
Defines operations for Biller entity:
- `GetAllAsync()`: Retrieve all billers
- `GetByIdAsync(Guid billerId)`: Get biller by ID
- `Insert(Biller biller)`: Add new biller
- `Remove(Biller biller)`: Delete biller

### IGenericRepository
Generic repository interface for common operations:
- `GetAllAsync()`: Retrieve all entities
- `GetByIdAsync(TKey id)`: Get entity by ID
- `Insert(T entity)`: Add new entity
- `Remove(T entity)`: Delete entity

### IRepositoryManager
Facade pattern that provides access to all repositories:
- `BillerRepository`: Access to biller operations
- `UnitOfWork`: Transaction management

### IUnitOfWork
Manages database transactions:
- `SaveChangesAsync()`: Persist changes to database

## Domain Exceptions

### Base Exceptions
- `BadRequestException`: Base class for 400-level errors
- `NotFoundException`: Base class for 404-level errors

### Specific Exceptions
- `BillerNotFoundException`: Thrown when biller is not found

## Benefits

1. **Testability**: Easy to unit test business logic in isolation
2. **Independence**: No coupling to external frameworks
3. **Clarity**: Clear business rules and domain concepts
4. **Flexibility**: Can be used with any infrastructure implementation

## Usage in Other Layers

- **Service Layer**: Implements business logic using domain entities and repositories
- **Infrastructure Layer**: Implements repository interfaces for data persistence
- **Presentation Layer**: Uses domain entities through service layer 