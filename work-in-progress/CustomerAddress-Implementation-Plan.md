# CustomerAddress Implementation Plan

## Overview
Implement a new CustomerAddress entity that allows customers to have multiple addresses, with full CRUD operations exposed via Swagger endpoints.

## Implementation Tasks

### 1. Domain Layer (Backend/Domain/)

- [x] **Create AddressType Enum**
  - **File**: `Backend/Domain/Enums/AddressType.cs`
  - **Values**: Billing, Shipping, Both

- [x] **Create CustomerAddress Entity**
  - **File**: `Backend/Domain/Entities/CustomerAddress.cs`
  - **Inherits**: `BaseEntity`
  - **Properties**:
    - `Id` (long, primary key)
    - `PublicId` (Guid, unique identifier)
    - `CustomerId` (long, foreign key to Customer)
    - `AddressType` (enum: Billing, Shipping, Both)
    - `StreetAddress1` (string, required)
    - `StreetAddress2` (string, optional)
    - `City` (string, required)
    - `State` (string, required)
    - `PostalCode` (string, required)
    - `Country` (string, required, default "US")
    - `IsDefault` (bool, indicates if this is the default address)
    - Navigation property: `Customer`

- [x] **Create CustomerAddressNotFoundException**
  - **File**: `Backend/Domain/Exceptions/CustomerAddressNotFoundException.cs`
  - **Purpose**: Throw when CustomerAddress not found by ID or PublicId

- [x] **Update Customer Entity**
  - **File**: `Backend/Domain/Entities/Customer.cs`
  - **Add**: `ICollection<CustomerAddress> Addresses { get; set; }`

- [x] **Update Biller Entity**
  - **File**: `Backend/Domain/Entities/Biller.cs`
  - **Add**: `ICollection<Customer> Customers { get; set; }`

- [x] **Update PaymentGateway Entity**
  - **File**: `Backend/Domain/Entities/PaymentGateway.cs`
  - **Add**: `ICollection<Customer> Customers { get; set; }`

### 2. Contracts Layer (Backend/Contracts/)

- [x] **Create CustomerAddressDto**
  - **File**: `Backend/Contracts/CustomerAddressDto.cs`
  - All properties from entity + audit fields

- [x] **Create CustomerAddressForCreationDto**
  - **File**: `Backend/Contracts/CustomerAddressForCreationDto.cs`
  - All properties except Id, PublicId, audit fields

- [x] **Create CustomerAddressForUpdateDto**
  - **File**: `Backend/Contracts/CustomerAddressForUpdateDto.cs`
  - All properties except Id, PublicId, audit fields (all optional)

### 3. Services Layer (Backend/Services/ & Backend/Services.Abstractions/)

- [x] **Create ICustomerAddressService Interface**
  - **File**: `Backend/Services.Abstractions/ICustomerAddressService.cs`
  - **Methods**:
    - `GetAllAsync(CancellationToken cancellationToken)`
    - `GetByIdAsync(long id, CancellationToken cancellationToken)`
    - `GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken)`
    - `GetByCustomerIdAsync(long customerId, CancellationToken cancellationToken)`
    - `CreateAsync(CustomerAddressForCreationDto dto, CancellationToken cancellationToken)`
    - `UpdateAsync(long id, CustomerAddressForUpdateDto dto, CancellationToken cancellationToken)`
    - `DeleteAsync(long id, CancellationToken cancellationToken)`

- [x] **Create CustomerAddressService Implementation**
  - **File**: `Backend/Services/CustomerAddressService.cs`
  - **Features**:
    - Business logic for setting default address
    - Validation for address types
    - Mapster for entity-DTO mapping
    - Use `IGenericRepository<CustomerAddress>` for data access

- [x] **Update IServiceManager Interface**
  - **File**: `Backend/Services.Abstractions/IServiceManager.cs`
  - **Add**: `ICustomerAddressService CustomerAddressService { get; }`

- [x] **Update ServiceManager Implementation**
  - **File**: `Backend/Services/ServiceManager.cs`
  - **Add**: `ICustomerAddressService CustomerAddressService { get; }`

- [x] **Update Service Registration**
  - **File**: `Backend/Services/ServiceCollectionExtensions.cs`
  - **Add**: `services.AddScoped<ICustomerAddressService, CustomerAddressService>();`

### 4. Persistence Layer (Backend/Persistence/)

- [ ] **Create CustomerAddressConfiguration**
  - **File**: `Backend/Persistence/Configurations/CustomerAddressConfiguration.cs`
  - **Features**:
    - Table name configuration
    - Property constraints (max lengths, required fields)
    - Foreign key relationship to Customer
    - Indexes on PublicId and CustomerId
    - Unique constraint on CustomerId + IsDefault combination

- [ ] **Update RepositoryDbContext**
  - **File**: `Backend/Persistence/RepositoryDbContext.cs`
  - **Add**: `DbSet<CustomerAddress> CustomerAddresses { get; set; }`

- [ ] **Create Database Migration**
  - **Command**: `dotnet ef migrations add AddCustomerAddressTable`
  - **Features**:
    - CustomerAddress table creation
    - Foreign key constraints
    - Indexes and unique constraints

### 5. Presentation Layer (Backend/Presentation/)

- [x] **Create CustomerAddressesController**
  - **File**: `Backend/Presentation/Controllers/CustomerAddressesController.cs`
  - **Endpoints**:
    - `GET /api/customer-addresses` - Get all addresses
    - `GET /api/customer-addresses/{id}` - Get by ID
    - `GET /api/customer-addresses/public/{publicId}` - Get by PublicId
    - `GET /api/customer-addresses/customers/{customerId}` - Get addresses by customer
    - `POST /api/customer-addresses` - Create new address
    - `PUT /api/customer-addresses/{id}` - Update address
    - `DELETE /api/customer-addresses/{id}` - Delete address

### 6. Dependency Injection Updates

- [ ] **Update Service Registration**
  - **File**: `Backend/Services/ServiceCollectionExtensions.cs`
  - **Add**: `services.AddScoped<ICustomerAddressService, CustomerAddressService>();`

### 7. Testing

- [ ] **Create Unit Tests**
  - **File**: `Backend/Payments.AcceptanceTests/CustomerAddressServiceTests.cs`
  - Test service layer business logic

- [ ] **Create Integration Tests**
  - **File**: `Backend/Payments.AcceptanceTests/CustomerAddressControllerTests.cs`
  - Test full API endpoint functionality

## Implementation Order

1. **Domain Layer** (Entity, Enum, Exception) ✅
2. **Contracts Layer** (DTOs) ✅
3. **Persistence Layer** (Configuration, DbContext updates)
4. **Database Migration**
5. **Services Layer** (Interface, Implementation, Service Manager updates)
6. **Dependency Injection** (Service registration)
7. **Presentation Layer** (Controller)
8. **Testing** (Unit tests and integration tests)

## Business Rules

1. **Default Address**: Only one address per customer can be marked as default
2. **Address Types**: Address can be Billing, Shipping, or Both
3. **Validation**: Required fields: StreetAddress1, City, State, PostalCode, Country
4. **Cascade Delete**: When customer is deleted, all their addresses are deleted
5. **PublicId**: Auto-generated GUID for external API access

## API Endpoints Summary

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/customer-addresses` | Get all addresses |
| GET | `/api/customer-addresses/{id}` | Get address by ID |
| GET | `/api/customer-addresses/public/{publicId}` | Get address by PublicId |
| GET | `/api/customers/{customerId}/addresses` | Get addresses for specific customer |
| POST | `/api/customer-addresses` | Create new address |
| PUT | `/api/customer-addresses/{id}` | Update existing address |
| DELETE | `/api/customer-addresses/{id}` | Delete address |

## Notes

- Follow existing patterns from Biller implementation
- Use Mapster for entity-DTO mapping
- Implement proper error handling and validation
- Add Swagger documentation attributes
- Use `IGenericRepository<CustomerAddress>` for data access (no custom repository needed)
- Consider adding address validation service for future enhancement