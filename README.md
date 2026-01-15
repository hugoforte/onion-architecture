# Starter Todo App Template

A **production-ready full-stack todo application template** built with:

- **Backend**: ASP.NET Core 9.0 with Onion Architecture
- **Frontend**: React 18 + TypeScript + Vite + Material UI
- **Database**: PostgreSQL
- **Infrastructure**: AWS ECS Fargate + Terraform IaC
- **Messaging**: NServiceBus with Learning Transport

This template demonstrates best practices for building enterprise-grade applications with clear separation of concerns, testability, and scalability.

## Quick Start

### Prerequisites

- .NET 9.0 SDK
- Node.js 22+
- Docker & Docker Compose (optional)
- PostgreSQL (or use Docker)

### Local Development

#### 1. Backend Setup

```powershell
cd Backend/3_Run/Web
dotnet restore
dotnet build
dotnet run
```

The API will be available at `http://localhost:5003` with Swagger at `http://localhost:5003/swagger`.

#### 2. Frontend Setup

```powershell
cd Frontend
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`.

#### 3. Docker Compose (Optional)

```powershell
cd Backend/3_Run/Docker
docker compose up -d
```

This starts:
- PostgreSQL database on port 5432
- API on port 5003

### Running Tests

```powershell
dotnet test Starter.sln
```

All tests (Unit + Acceptance) must pass before deployment.

## Project Structure

```
Template/
├── Backend/
│   ├── 0_Core/                # Domain layer (no dependencies)
│   │   ├── Common/            # Shared utilities
│   │   ├── Contracts/         # DTOs and API contracts
│   │   ├── Domain/            # Entities, repositories, exceptions
│   │   └── Messaging/         # NServiceBus messages
│   ├── 1_Infrastructure/      # Infrastructure layer
│   │   └── Infrastructure/    # DB context, repositories, configs
│   ├── 2_Application/         # Business logic layer
│   │   ├── Services/          # Service implementations
│   │   └── Services.Abstractions/ # Service interfaces
│   ├── 3_Run/                 # Runtime layer
│   │   ├── Web/              # ASP.NET Core host, controllers
│   │   ├── ServiceBus/       # NServiceBus hosting
│   │   └── Docker/           # Docker Compose config
│   └── Tests/                # Unit and acceptance tests
├── Frontend/                  # React TypeScript app
├── Deploy/                    # Terraform infrastructure
└── README.md                  # This file
```

## Architecture

### Onion Architecture (Clean Architecture)

Dependencies flow **inward only**:

```
3_Run (Web) → 2_Application → 1_Infrastructure → 0_Core (Domain)
```

**Benefits:**
- Independent, testable business logic
- Flexible external dependencies
- Easy to maintain and extend
- Framework-agnostic core

### Tech Stack Details

**Backend:**
- **Framework**: ASP.NET Core 9.0
- **Database**: Entity Framework Core 9.0 + Npgsql
- **Patterns**: Repository, Unit of Work, Dependency Injection
- **Validation**: Data Annotations
- **Mapping**: Mapster
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI
- **Messaging**: NServiceBus with Learning Transport
- **Testing**: NUnit + Moq + AutoFixture + Shouldly

**Frontend:**
- **UI Framework**: React 18
- **Language**: TypeScript
- **Build Tool**: Vite
- **Component Library**: Material UI v6
- **Data Fetching**: TanStack Query (React Query)
- **State Management**: Zustand
- **HTTP Client**: Axios

## API Endpoints

### Todo Lists

- `GET /api/todo-lists` - Get all lists
- `GET /api/todo-lists/{id}` - Get list by ID
- `POST /api/todo-lists` - Create new list
- `PUT /api/todo-lists/{id}` - Update list
- `DELETE /api/todo-lists/{id}` - Delete list

### Todo Items

- `GET /api/todo-lists/{listId}/items` - Get items for list
- `POST /api/todo-items` - Create new item
- `PUT /api/todo-items/{id}` - Update item
- `PUT /api/todo-items/{id}/complete` - Mark as complete
- `DELETE /api/todo-items/{id}` - Delete item

## Development Workflow

### Adding a New Feature

1. **Define Domain** - Create entities in `0_Core/Domain`
2. **Create DTOs** - Add request/response contracts in `0_Core/Contracts`
3. **Implement Repository** - Add repository in `1_Infrastructure`
4. **Implement Service** - Add business logic in `2_Application`
5. **Create Controller** - Add API endpoint in `3_Run/Web`
6. **Write Tests** - Add unit and acceptance tests in `Tests`
7. **Create Migration** - If schema changes, run `dotnet ef migrations add`

### Creating a Database Migration

```powershell
cd Backend
dotnet tool run dotnet-ef migrations add <MigrationName> `
    --project 1_Infrastructure/Infrastructure `
    --startup-project 3_Run/Web `
    --output-dir Database/Migrations
```

### Running Tests

```powershell
# All tests
dotnet test Starter.sln

# Specific test file
dotnet test Backend/Tests/UnitTests/Services/TodoItemServiceTests.cs

# With coverage
dotnet test Starter.sln /p:CollectCoverage=true
```

## Configuration

### Backend Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Database=starter;Username=postgres;Password=postgres"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" }
    ]
  }
}
```

### Frontend Environment Variables

Create `.env` in `Frontend/`:

```
VITE_API_BASE_URL=http://localhost:5003
```

## Deployment

### Docker Build & Push

```bash
# Build image
docker build -t starter-api:latest Backend/3_Run/Web

# Tag for ECR
docker tag starter-api:latest <ACCOUNT>.dkr.ecr.us-east-1.amazonaws.com/starter-todo-app/starter-api:latest

# Push to ECR
docker push <ACCOUNT>.dkr.ecr.us-east-1.amazonaws.com/starter-todo-app/starter-api:latest
```

### Terraform Deployment

See [Deploy/README.md](Deploy/README.md) for detailed AWS deployment instructions.

```bash
cd Deploy

# Development
terraform apply -var-file=environments/dev/terraform.tfvars

# UAT
terraform apply -var-file=environments/uat/terraform.tfvars

# Production
terraform apply -var-file=environments/prod/terraform.tfvars
```

## Customization

### Rename Template

To customize this template for your project, use the provided PowerShell script:

```powershell
# From template root
.\Rename-Template.ps1 -NewProjectName "MyAwesomeApp" -NewNamespace "MyCompany.MyApp"
```

This script will:
- Rename all projects
- Update namespaces
- Update folder structure
- Update solution file references
- Update Docker image names

## Troubleshooting

### Database Connection Errors

Ensure PostgreSQL is running and connection string is correct:

```powershell
# Using Docker Compose
docker compose -f Backend/3_Run/Docker/docker-compose.yml up -d postgres

# Check logs
docker compose logs postgres
```

### API Not Responding

```powershell
# Check API is running
netstat -ano | findstr :5003

# Check Swagger
curl http://localhost:5003/swagger
```

### Frontend API Calls Failing

- Verify backend is running on port 5003
- Check CORS configuration in `Program.cs`
- Verify `VITE_API_BASE_URL` environment variable

## Best Practices

### Code Organization

- Keep controllers thin - delegate to services
- Place business logic in services
- Use dependency injection for all dependencies
- Keep domain logic in entities

### Database

- Use migrations for schema changes
- Always use async/await with database calls
- Use `CancellationToken` for graceful shutdown
- Index frequently queried columns

### Frontend

- Use React hooks (no class components)
- Keep components small and reusable
- Use TanStack Query for server state
- Use Zustand for client state

### Testing

- Write unit tests for services
- Write acceptance tests for APIs
- Aim for >80% code coverage
- Test edge cases and error scenarios

## Key Files

- [Backend/README.md](Backend/README.md) - Backend-specific documentation
- [Frontend/README.md](Frontend/README.md) - Frontend-specific documentation
- [Deploy/README.md](Deploy/README.md) - Infrastructure documentation
- `Directory.Build.props` - Global build properties
- `Directory.Packages.props` - Centralized package versions

## Support & Contributing

This is a template repository. Customize it for your needs by:

1. Cloning/copying this template
2. Running `Rename-Template.ps1` to customize project names
3. Adding your business logic and domain entities
4. Updating API endpoints and frontend components

## License

This template is provided as-is for educational and commercial use.

## Next Steps

1. ✅ Backend with tests passing
2. ✅ Frontend setup with Material UI
3. ✅ Docker Compose for local development
4. ✅ Terraform infrastructure
5. → Customize namespaces with `Rename-Template.ps1`
6. → Add your business logic
7. → Deploy to AWS

Happy coding! 🚀
