# Todo App Backend

ASP.NET Core 9.0 backend with Onion Architecture, Entity Framework Core, NServiceBus messaging, and comprehensive tests.

## Tech Stack

- **Framework**: ASP.NET Core 9.0
- **ORM**: Entity Framework Core 9.0
- **Database**: PostgreSQL 16 (with SQLite for testing)
- **Patterns**: Repository, Unit of Work, Dependency Injection
- **Mapping**: Mapster
- **Logging**: Serilog
- **API Docs**: Swagger/OpenAPI
- **Messaging**: NServiceBus with Learning Transport
- **Testing**: NUnit, Moq, AutoFixture, Shouldly

## Architecture

### Onion Architecture Layers

```
Dependencies flow inward only ↓

3_Run (Web)
  ↓
2_Application (Services)
  ↓
1_Infrastructure (Database, Repositories)
  ↓
0_Core (Domain, Contracts, Exceptions)
```

### Project Structure

```
Backend/
├── 0_Core/
│   ├── Common/                  # Shared utilities (Constants, Utils)
│   │   ├── Configuration/       # Options pattern configs
│   │   ├── Constants/           # App-wide constants
│   │   └── Utils/              # Helper utilities
│   ├── Contracts/              # DTOs for API communication
│   │   ├── Requests/           # *ForCreationDto, *ForUpdateDto
│   │   └── Responses/          # *Dto response objects
│   ├── Domain/                 # Business domain
│   │   ├── Entities/           # Business entities (TodoList, TodoItem)
│   │   ├── Repositories/       # Repository interfaces
│   │   ├── Exceptions/         # Domain exceptions
│   │   └── Enums/              # Domain enums
│   └── Messaging/              # NServiceBus messages
│       ├── Commands/           # Message commands
│       └── Events/             # Message events
│
├── 1_Infrastructure/
│   └── Infrastructure/         # EF Core implementation
│       ├── Database/
│       │   ├── Configurations/ # EF entity configurations
│       │   ├── Migrations/     # EF migrations
│       │   └── RepositoryDbContext.cs
│       ├── Database/Repositories/  # Repository implementations
│       ├── Extensions/         # DI extension methods
│       └── UnitOfWork.cs       # Unit of Work pattern
│
├── 2_Application/
│   ├── Services.Abstractions/  # Service interfaces
│   │   └── I*Service.cs        # Service contracts
│   └── Services/               # Service implementations
│       ├── TodoListService.cs
│       ├── TodoItemService.cs
│       └── ServiceManager.cs
│
├── 3_Run/
│   ├── Web/
│   │   ├── Controllers/        # API endpoints
│   │   ├── Middleware/         # Custom middleware
│   │   ├── Extensions/         # DI registration
│   │   ├── Program.cs          # Entry point
│   │   ├── appsettings.json    # Configuration
│   │   └── Dockerfile          # Container definition
│   │
│   └── ServiceBus/
│       ├── Handlers/           # Message handlers
│       ├── Program.cs          # NServiceBus endpoint
│       └── appsettings.json    # Messaging config
│
└── Tests/
    ├── UnitTests/              # Service unit tests
    ├── AcceptanceTests/        # API acceptance tests
    └── Shared/                 # Test utilities
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 16 (or Docker)

### Setup

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run migrations
dotnet ef database update \
  --project Backend/1_Infrastructure/Infrastructure \
  --startup-project Backend/3_Run/Web
```

### Run Locally

```bash
cd Backend/3_Run/Web
dotnet run
```

API: `http://localhost:5003`
Swagger: `http://localhost:5003/swagger`

### Run with Docker Compose

```bash
cd Backend/3_Run/Docker
docker compose up -d
```

## Code Conventions

### Project References

Always follow the architecture:
- 3_Run → 2_Application, 1_Infrastructure
- 2_Application → 0_Core, 1_Infrastructure
- 1_Infrastructure → 0_Core
- 0_Core → (nothing)

**Never have circular dependencies!**

### Naming Conventions

#### Entities
- PascalCase: `TodoList`, `TodoItem`
- Inherit from `BaseEntity<T>` where T is the key type

#### DTOs
- Response: `TodoListDto`, `TodoItemDto`
- Creation: `TodoListForCreationDto`, `TodoItemForCreationDto`
- Update: `TodoListForUpdateDto`, `TodoItemForUpdateDto`

#### Services
- Interface: `ITodoListService`
- Implementation: `internal sealed class TodoListService : ITodoListService`
- Manager: `IServiceManager`, `ServiceManager`

#### Repositories
- Interface: `ITodoListRepository : IGenericRepository<TodoList, Guid>`
- Implementation: `internal sealed class TodoListRepository : GenericRepository<TodoList, Guid>, ITodoListRepository`

### Class Accessibility

```csharp
// ✅ Service implementations are internal sealed
internal sealed class TodoListService : ITodoListService
{
    private readonly IRepositoryManager _repositoryManager;
    
    public TodoListService(IRepositoryManager repositoryManager)
        => _repositoryManager = repositoryManager;
}

// ❌ Don't do this
public class TodoListService : ITodoListService { }
```

### Async Methods

All I/O operations must be async with CancellationToken:

```csharp
// ✅ Correct
public async Task<TodoListDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
{
    var todoList = await _repositoryManager.TodoList.GetByIdAsync(id, cancellationToken);
    return _mapper.Map<TodoListDto>(todoList);
}

// ❌ Never synchronous I/O
public TodoListDto GetById(Guid id)
{
    var todoList = _repositoryManager.TodoList.GetById(id);
    return _mapper.Map<TodoListDto>(todoList);
}
```

### Entity Configuration

Use EF Core fluent API in `Configuration` classes:

```csharp
public class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasMany(x => x.Items)
            .WithOne(x => x.TodoList)
            .HasForeignKey(x => x.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### Exception Handling

Use domain exceptions, never catch all:

```csharp
// ✅ Specific exceptions
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }
}

// ❌ Generic exceptions
throw new Exception("Not found");
```

## Database

### Migrations

Create migration:
```bash
dotnet ef migrations add AddTodoItems \
  --project Backend/1_Infrastructure/Infrastructure \
  --startup-project Backend/3_Run/Web \
  --output-dir Database/Migrations
```

Apply migration:
```bash
dotnet ef database update \
  --project Backend/1_Infrastructure/Infrastructure \
  --startup-project Backend/3_Run/Web
```

### Connection Strings

**Development (appsettings.json):**
```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Database=starter;Username=postgres;Password=postgres;"
  }
}
```

**Docker Compose:**
```
Host=postgres;Port=5432;Database=starter;Username=postgres;Password=postgres;
```

**Environment Variable:**
```bash
DB_CONNECTION_STRING="Host=mydb.com;Port=5432;Database=starter;..."
```

## API Endpoints

### Todo Lists

```
GET    /api/todo-lists              → Get all lists
GET    /api/todo-lists/{id}         → Get list by ID
POST   /api/todo-lists              → Create list
PUT    /api/todo-lists/{id}         → Update list
DELETE /api/todo-lists/{id}         → Delete list
```

Request:
```json
{
  "name": "Shopping List",
  "description": "Weekly groceries"
}
```

Response:
```json
{
  "id": "guid",
  "name": "Shopping List",
  "description": "Weekly groceries",
  "createdAt": "2024-01-14T10:00:00Z",
  "updatedAt": "2024-01-14T10:00:00Z"
}
```

### Todo Items

```
GET    /api/todo-lists/{listId}/items    → Get items in list
POST   /api/todo-items                   → Create item
PUT    /api/todo-items/{id}              → Update item
PUT    /api/todo-items/{id}/complete     → Mark complete
DELETE /api/todo-items/{id}              → Delete item
```

## Testing

### Run All Tests

```bash
dotnet test Starter.sln
```

### Run Unit Tests Only

```bash
dotnet test Backend/Tests/UnitTests/
```

### Run Acceptance Tests Only

```bash
dotnet test Backend/Tests/AcceptanceTests/
```

### Test Coverage

```bash
dotnet test Starter.sln /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Unit Test Example

```csharp
[TestFixture]
public class TodoListServiceTests
{
    private Mock<IRepositoryManager> _repositoryManagerMock;
    private TodoListService _sut;
    
    [SetUp]
    public void Setup()
    {
        _repositoryManagerMock = new Mock<IRepositoryManager>();
        _sut = new TodoListService(_repositoryManagerMock.Object);
    }
    
    [Test]
    public async Task GetByIdAsync_WhenListExists_ReturnsListDto()
    {
        // Arrange
        var fixture = new Fixture();
        var todoList = fixture.Create<TodoList>();
        
        _repositoryManagerMock
            .Setup(x => x.TodoList.GetByIdAsync(todoList.Id, default))
            .ReturnsAsync(todoList);
        
        // Act
        var result = await _sut.GetByIdAsync(todoList.Id);
        
        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(todoList.Name);
        
        _repositoryManagerMock.Verify(
            x => x.TodoList.GetByIdAsync(todoList.Id, default),
            Times.Once);
    }
}
```

### Acceptance Test Example

```csharp
[TestFixture]
public class TodoApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _httpClient;
    
    [OneTimeSetUp]
    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _httpClient = _factory.CreateClient();
    }
    
    [Test]
    public async Task CreateTodoList_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new CreateTodoListDto 
        { 
            Name = "Test List",
            Description = "Test Description"
        };
        
        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/todo-lists", request);
        
        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
    }
    
    [OneTimeTearDown]
    public async Task DisposeAsync()
    {
        _httpClient?.Dispose();
        _factory?.Dispose();
    }
}
```

## Messaging with NServiceBus

### Publishing Events

```csharp
public class TodoItemService : ITodoItemService
{
    private readonly IMessageSession _messageSession;
    
    public async Task CompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _repositoryManager.TodoItem.GetByIdAsync(id, cancellationToken);
        item.IsCompleted = true;
        
        // Publish event
        var @event = new TodoItemCompletedEvent { TodoItemId = id };
        await _messageSession.Publish(@event, cancellationToken);
    }
}
```

### Handling Events

```csharp
public class TodoItemCompletedHandler : IHandleMessages<TodoItemCompletedEvent>
{
    private readonly ILogger<TodoItemCompletedHandler> _logger;
    
    public Task Handle(TodoItemCompletedEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation($"Todo item {message.TodoItemId} completed!");
        return Task.CompletedTask;
    }
}
```

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Database=starter;Username=postgres;Password=postgres;"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Ansi"
        }
      }
    ]
  }
}
```

### Options Pattern

```csharp
// Configuration class in 0_Core/Common/Configuration
public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}

// Registration in Program.cs
services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

// Usage in service
public class AuthService
{
    private readonly JwtOptions _options;
    
    public AuthService(IOptions<JwtOptions> options)
        => _options = options.Value;
}
```

## Dependency Injection

All dependencies are registered in DI container. Add services in Program.cs:

```csharp
// Infrastructure
services.AddInfrastructure(configuration);

// Application
services.AddApplication();

// Add controllers
services.AddControllers();

// Add Swagger
services.AddSwaggerGen();
```

## Logging

Use Serilog for structured logging:

```csharp
private readonly ILogger<TodoListService> _logger;

public async Task<TodoListDto> CreateAsync(CreateTodoListDto dto, CancellationToken cancellationToken = default)
{
    _logger.LogInformation("Creating todo list: {Name}", dto.Name);
    
    try
    {
        // ... create list
        _logger.LogInformation("Todo list created: {ListId}", list.Id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating todo list");
        throw;
    }
}
```

## Deployment

### Build Docker Image

```bash
docker build -t starter-api:latest Backend/3_Run/Web
```

### Run Container

```bash
docker run -p 5003:5003 \
  -e ConnectionStrings__Database="Host=postgres;Port=5432;Database=starter;..." \
  starter-api:latest
```

### Deploy to AWS ECS

See [Deploy/README.md](../Deploy/README.md) for Terraform-based AWS deployment.

## Performance Tips

1. **Use async/await** for all I/O operations
2. **Index frequently queried columns** in the database
3. **Use projection queries** to fetch only needed columns
4. **Cache read-only data** with Redis (for future enhancement)
5. **Batch database operations** when possible
6. **Use paging** for large result sets

## Security

1. **Always validate input** - Use Data Annotations on DTOs
2. **Use parameterized queries** - EF Core does this by default
3. **Never expose stack traces** - Use global exception handling
4. **Validate authorization** - Add [Authorize] attributes
5. **Use HTTPS** in production
6. **Encrypt sensitive data** - Use AWS Secrets Manager

## Troubleshooting

### Build Errors

```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Test Failures

```bash
# Run with verbose output
dotnet test --logger "console;verbosity=detailed"

# Run specific test
dotnet test --filter "FullyQualifiedName~TodoListServiceTests.GetByIdAsync"
```

### Database Errors

```bash
# Check connection
dotnet ef dbcontext validate \
  --project Backend/1_Infrastructure/Infrastructure \
  --startup-project Backend/3_Run/Web

# Revert migration
dotnet ef migrations remove \
  --project Backend/1_Infrastructure/Infrastructure \
  --startup-project Backend/3_Run/Web
```

## References

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [NServiceBus Documentation](https://docs.particular.net/nservicebus/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
