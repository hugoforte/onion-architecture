# Payments.Presentation

## Overview

The **Presentation** project contains the API controllers and handles HTTP requests/responses. It represents the **Presentation Layer** in the Onion Architecture and is responsible for exposing the business logic through RESTful API endpoints.

## Responsibilities

- **API Controllers**: Handle HTTP requests and responses
- **Request/Response Mapping**: Convert between HTTP and business layer
- **Input Validation**: Validate incoming requests
- **Error Handling**: Handle and format error responses
- **API Documentation**: Provide clear API contracts

## Project Structure

```
Payments.Presentation/
├── Controllers/                    # API controllers
│   ├── BillersController.cs       # Biller-related endpoints
│   ├── CustomersController.cs     # Customer-related endpoints
│   └── InvoicesController.cs      # Invoice-related endpoints
└── AssemblyReference.cs           # Assembly reference for DI
```

## API Controllers

### BillersController
Handles all biller-related HTTP operations:

```csharp
[ApiController]
[Route("api/billers")]
public class BillersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public BillersController(IServiceManager serviceManager) => _serviceManager = serviceManager;
}
```

**Endpoints:**

1. **GET /api/billers** - Retrieve all billers
   ```csharp
   [HttpGet]
   public async Task<IActionResult> GetBillers(CancellationToken cancellationToken)
   {
       var billers = await _serviceManager.BillerService.GetAllAsync(cancellationToken);
       return Ok(billers);
   }
   ```

2. **GET /api/billers/{billerId}** - Get biller by ID
   ```csharp
   [HttpGet("{billerId:guid}")]
   public async Task<IActionResult> GetBillerById(Guid billerId, CancellationToken cancellationToken)
   {
       var billerDto = await _serviceManager.BillerService.GetByIdAsync(billerId, cancellationToken);
       return Ok(billerDto);
   }
   ```

3. **POST /api/billers** - Create new biller
   ```csharp
   [HttpPost]
   public async Task<IActionResult> CreateBiller([FromBody] BillerForCreationDto billerForCreationDto)
   {
       var billerDto = await _serviceManager.BillerService.CreateAsync(billerForCreationDto);
       return CreatedAtAction(nameof(GetBillerById), new { billerId = billerDto.Id }, billerDto);
   }
   ```

4. **PUT /api/billers/{billerId}** - Update existing biller
   ```csharp
   [HttpPut("{billerId:guid}")]
   public async Task<IActionResult> UpdateBiller(Guid billerId, [FromBody] BillerForUpdateDto billerForUpdateDto, CancellationToken cancellationToken)
   {
       await _serviceManager.BillerService.UpdateAsync(billerId, billerForUpdateDto, cancellationToken);
       return NoContent();
   }
   ```

5. **DELETE /api/billers/{billerId}** - Delete biller
   ```csharp
   [HttpDelete("{billerId:guid}")]
   public async Task<IActionResult> DeleteBiller(Guid billerId, CancellationToken cancellationToken)
   {
       await _serviceManager.BillerService.DeleteAsync(billerId, cancellationToken);
       return NoContent();
   }
   ```

### CustomersController
Handles all customer-related HTTP operations:

```csharp
[ApiController]
[Route("api/billers/{billerId:guid}/customers")]
public class CustomersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public CustomersController(IServiceManager serviceManager) => _serviceManager = serviceManager;
}
```

**Endpoints:**

1. **GET /api/billers/{billerId}/customers** - Get all customers for biller
   ```csharp
   [HttpGet]
   public async Task<IActionResult> GetCustomers(Guid billerId, CancellationToken cancellationToken)
   {
       var customersDto = await _serviceManager.CustomerService.GetAllByBillerIdAsync(billerId, cancellationToken);
       return Ok(customersDto);
   }
   ```

2. **GET /api/billers/{billerId}/customers/{customerId}** - Get specific customer
   ```csharp
   [HttpGet("{customerId:guid}")]
   public async Task<IActionResult> GetCustomerById(Guid billerId, Guid customerId, CancellationToken cancellationToken)
   {
       var customerDto = await _serviceManager.CustomerService.GetByIdAsync(billerId, customerId, cancellationToken);
       return Ok(customerDto);
   }
   ```

3. **POST /api/billers/{billerId}/customers** - Create new customer for biller
   ```csharp
   [HttpPost]
   public async Task<IActionResult> CreateCustomer(Guid billerId, [FromBody] CustomerForCreationDto customerForCreationDto, CancellationToken cancellationToken)
   {
       var response = await _serviceManager.CustomerService.CreateAsync(billerId, customerForCreationDto, cancellationToken);
       return CreatedAtAction(nameof(GetCustomerById), new { billerId = billerId, customerId = response.Id }, response);
   }
   ```

4. **PUT /api/billers/{billerId}/customers/{customerId}** - Update customer
   ```csharp
   [HttpPut("{customerId:guid}")]
   public async Task<IActionResult> UpdateCustomer(Guid billerId, Guid customerId, [FromBody] CustomerForUpdateDto customerForUpdateDto, CancellationToken cancellationToken)
   {
       await _serviceManager.CustomerService.UpdateAsync(billerId, customerId, customerForUpdateDto, cancellationToken);
       return NoContent();
   }
   ```

5. **DELETE /api/billers/{billerId}/customers/{customerId}** - Delete customer
   ```csharp
   [HttpDelete("{customerId:guid}")]
   public async Task<IActionResult> DeleteCustomer(Guid billerId, Guid customerId, CancellationToken cancellationToken)
   {
       await _serviceManager.CustomerService.DeleteAsync(billerId, customerId, cancellationToken);
       return NoContent();
   }
   ```

### InvoicesController
Handles all invoice-related HTTP operations:

```csharp
[ApiController]
[Route("api/billers/{billerId:guid}/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public InvoicesController(IServiceManager serviceManager) => _serviceManager = serviceManager;
}
```

**Endpoints:**

1. **GET /api/billers/{billerId}/invoices** - Get all invoices for biller
   ```csharp
   [HttpGet]
   public async Task<IActionResult> GetInvoices(Guid billerId, CancellationToken cancellationToken)
   {
       var invoicesDto = await _serviceManager.InvoiceService.GetAllByBillerIdAsync(billerId, cancellationToken);
       return Ok(invoicesDto);
   }
   ```

2. **GET /api/billers/{billerId}/invoices/{invoiceId}** - Get specific invoice
   ```csharp
   [HttpGet("{invoiceId:guid}")]
   public async Task<IActionResult> GetInvoiceById(Guid billerId, Guid invoiceId, CancellationToken cancellationToken)
   {
       var invoiceDto = await _serviceManager.InvoiceService.GetByIdAsync(billerId, invoiceId, cancellationToken);
       return Ok(invoiceDto);
   }
   ```

3. **POST /api/billers/{billerId}/invoices** - Create new invoice
   ```csharp
   [HttpPost]
   public async Task<IActionResult> CreateInvoice(Guid billerId, [FromBody] InvoiceForCreationDto invoiceForCreationDto, CancellationToken cancellationToken)
   {
       var response = await _serviceManager.InvoiceService.CreateAsync(billerId, invoiceForCreationDto.CustomerId, invoiceForCreationDto, cancellationToken);
       return CreatedAtAction(nameof(GetInvoiceById), new { billerId = billerId, invoiceId = response.Id }, response);
   }
   ```

## Key Features

### 1. **RESTful Design**
Follows REST principles with proper HTTP methods and status codes.

### 2. **Dependency Injection**
Controllers receive service dependencies through constructor injection.

### 3. **Async Operations**
All controller actions are asynchronous for better performance.

### 4. **Cancellation Support**
Supports cancellation tokens for proper request handling.

### 5. **Input Validation**
Uses data annotations and model binding for request validation.

### 6. **Proper HTTP Status Codes**
- `200 OK`: Successful GET operations
- `201 Created`: Successful POST operations
- `204 No Content`: Successful DELETE/PUT operations
- `400 Bad Request`: Validation errors
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server errors

## API Documentation

### Swagger Integration
The API is documented using Swagger/OpenAPI, accessible at:
```
https://localhost:5001/swagger
```

### Request/Response Examples

#### Create Biller Request
```json
POST /api/billers
{
  "name": "Acme Corporation",
  "apiKey": "sk_test_1234567890abcdef"
}
```

#### Create Customer Request
```json
POST /api/billers/{billerId}/customers
{
  "name": "John Doe",
  "autopayEnabled": true
}
```

#### Create Invoice Request
```json
POST /api/billers/{billerId}/invoices
{
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "dueDate": "2024-02-01T00:00:00Z",
  "totalAmount": 299.99,
  "currency": "USD",
  "status": "pending"
}
```

## Error Handling

### Global Exception Handling
Errors are handled by the `ExceptionHandlingMiddleware` in the Web project:

- `BadRequestException` → 400 Bad Request
- `NotFoundException` → 404 Not Found
- Other exceptions → 500 Internal Server Error

### Error Response Format
```json
{
  "error": "The biller with the identifier {id} was not found."
}
```

## Dependencies

- **Payments.Services.Abstractions**: Uses service interfaces
- **Payments.Contracts**: Uses DTOs for request/response
- **ASP.NET Core MVC**: Framework for API controllers

## Design Patterns

### 1. **Dependency Injection**
Controllers receive dependencies through constructor injection.

### 2. **Service Layer Pattern**
Controllers delegate business logic to service layer.

### 3. **DTO Pattern**
Uses DTOs for data transfer between layers.

### 4. **RESTful Design**
Follows REST principles for API design.

## Benefits

1. **Separation of Concerns**: Controllers focus only on HTTP handling
2. **Testability**: Easy to unit test controllers with mocked services
3. **Maintainability**: Clear API structure and documentation
4. **Scalability**: Async operations support high concurrency
5. **Standards Compliance**: Follows REST and HTTP standards

## Best Practices

1. **Keep Controllers Thin**: Delegate business logic to service layer
2. **Use Async/Await**: All operations should be asynchronous
3. **Proper HTTP Status Codes**: Return appropriate status codes
4. **Input Validation**: Validate all incoming requests
5. **Error Handling**: Handle exceptions gracefully
6. **API Documentation**: Provide clear API documentation

## Testing

Controllers can be easily unit tested:

```csharp
[Test]
public async Task GetBillers_ReturnsOkResult()
{
    // Arrange
    var mockServiceManager = new Mock<IServiceManager>();
    var mockBillerService = new Mock<IBillerService>();
    var billers = new List<BillerDto> { new BillerDto { Id = Guid.NewGuid(), Name = "Test" } };
    
    mockBillerService.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(billers);
    mockServiceManager.Setup(x => x.BillerService).Returns(mockBillerService.Object);
    
    var controller = new BillersController(mockServiceManager.Object);
    
    // Act
    var result = await controller.GetBillers();
    
    // Assert
    Assert.IsInstanceOf<OkObjectResult>(result);
}
```