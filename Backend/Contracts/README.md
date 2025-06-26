# Payments.Contracts

## Overview

The **Contracts** project contains Data Transfer Objects (DTOs) that define the structure of data exchanged between the API and external clients. These DTOs are part of the Domain layer and represent the contracts for data communication.

## Responsibilities

- **Data Transfer Objects (DTOs)**: Define the structure of data sent to and received from the API
- **API Contracts**: Specify the shape of request and response data
- **Validation Attributes**: Define validation rules for incoming data
- **Separation of Concerns**: Keep domain entities separate from API contracts

## Project Structure

```
Payments.Contracts/
├── BillerDto.cs              # Biller data for API responses
├── BillerForCreationDto.cs   # Biller data for creation requests
├── BillerForUpdateDto.cs     # Biller data for update requests
├── CustomerDto.cs            # Customer data for API responses
├── CustomerForCreationDto.cs # Customer data for creation requests
├── CustomerForUpdateDto.cs   # Customer data for update requests
├── InvoiceDto.cs             # Invoice data for API responses
├── InvoiceForCreationDto.cs  # Invoice data for creation requests
└── InvoiceForUpdateDto.cs    # Invoice data for update requests
```

## DTOs Overview

### BillerDto
Represents biller data in API responses:
```csharp
public class BillerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ApiKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### BillerForCreationDto
Defines data required to create a new biller with validation:
```csharp
public class BillerForCreationDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "API key is required")]
    [StringLength(255, ErrorMessage = "API key cannot be longer than 255 characters")]
    public string ApiKey { get; set; }
}
```

### BillerForUpdateDto
Defines data for updating an existing biller:
```csharp
public class BillerForUpdateDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "API key is required")]
    [StringLength(255, ErrorMessage = "API key cannot be longer than 255 characters")]
    public string ApiKey { get; set; }
}
```

### CustomerDto
Represents customer data in API responses:
```csharp
public class CustomerDto
{
    public Guid Id { get; set; }
    public Guid BillerId { get; set; }
    public string Name { get; set; }
    public bool AutopayEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### CustomerForCreationDto
Defines data required to create a new customer:
```csharp
public class CustomerForCreationDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
    public string Name { get; set; }

    public bool AutopayEnabled { get; set; } = false;
}
```

### InvoiceDto
Represents invoice data in API responses:
```csharp
public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid BillerId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

## Key Principles

### 1. **Separation from Domain Entities**
DTOs are separate from domain entities to avoid exposing internal structure and allow for API evolution.

### 2. **Input Validation**
Use data annotations to define validation rules for incoming data.

### 3. **API Versioning Support**
DTOs can be versioned independently of domain entities.

### 4. **Security**
Control what data is exposed to external clients.

## Validation Attributes

The DTOs use various validation attributes:

- `[Required]`: Ensures the property has a value
- `[StringLength]`: Limits the length of string properties
- Custom error messages for better user experience

## Benefits

1. **API Stability**: Changes to domain entities don't affect API contracts
2. **Security**: Control over what data is exposed
3. **Validation**: Centralized validation rules
4. **Documentation**: Clear contract for API consumers
5. **Flexibility**: Can evolve API independently of domain

## Usage

### In Service Layer
DTOs are used by services to:
- Accept creation/update requests
- Return data to controllers
- Map between domain entities and API contracts

### In Presentation Layer
Controllers use DTOs to:
- Define action method parameters
- Return API responses
- Validate incoming data

### Mapping
DTOs are mapped to/from domain entities using libraries like Mapster or AutoMapper.

## Best Practices

1. **Keep DTOs Simple**: Focus on data transfer, not business logic
2. **Use Validation**: Apply appropriate validation attributes
3. **Versioning**: Consider API versioning strategies
4. **Documentation**: Provide clear documentation for API consumers
5. **Consistency**: Maintain consistent naming and structure 