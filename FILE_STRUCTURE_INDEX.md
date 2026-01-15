# Template Complete File Structure & Index

## Root Level Files

```
Template/
├── README.md                          # Main project overview and quick start
├── GETTING_STARTED.md                # 5-minute quick start guide
├── TEMPLATE_SUMMARY.md               # Implementation summary and status
├── DELIVERABLES_CHECKLIST.md         # Complete checklist of all deliverables
├── Rename-Template.ps1               # PowerShell script to customize template
├── Starter.sln                       # Visual Studio solution file
├── Directory.Build.props             # Global build properties
├── Directory.Packages.props          # Central NuGet package versions
└── LICENSE                           # License file
```

## Backend Directory Structure

```
Backend/
├── 0_Core/                          # Domain layer (no dependencies)
│   ├── Common/
│   │   ├── Starter.Common.csproj
│   │   ├── Configuration/
│   │   ├── Constants/
│   │   └── Utils/
│   ├── Contracts/                   # DTOs for API
│   │   ├── Starter.Contracts.csproj
│   │   ├── Requests/
│   │   │   ├── CreateTodoListForCreationDto.cs
│   │   │   └── CreateTodoItemForCreationDto.cs
│   │   └── Responses/
│   │       ├── TodoListDto.cs
│   │       └── TodoItemDto.cs
│   ├── Domain/                      # Business domain
│   │   ├── Starter.Domain.csproj
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── TodoList.cs
│   │   │   └── TodoItem.cs
│   │   ├── Repositories/
│   │   │   ├── IGenericRepository.cs
│   │   │   ├── ITodoListRepository.cs
│   │   │   ├── ITodoItemRepository.cs
│   │   │   ├── IRepositoryManager.cs
│   │   │   └── IUnitOfWork.cs
│   │   └── Exceptions/
│   │       ├── DomainException.cs
│   │       ├── NotFoundException.cs
│   │       └── BadRequestException.cs
│   └── Messaging/                   # NServiceBus messages
│       ├── Starter.Messaging.csproj
│       ├── Commands/
│       │   └── CreateTodoItemCommand.cs
│       └── Events/
│           └── TodoItemCompletedEvent.cs
│
├── 1_Infrastructure/                # Infrastructure layer
│   └── Infrastructure/
│       ├── Starter.Infrastructure.csproj
│       ├── Database/
│       │   ├── RepositoryDbContext.cs
│       │   ├── DbContextFactory.cs
│       │   ├── Configurations/
│       │   │   ├── TodoListConfiguration.cs
│       │   │   └── TodoItemConfiguration.cs
│       │   ├── Repositories/
│       │   │   ├── GenericRepository.cs
│       │   │   ├── TodoListRepository.cs
│       │   │   ├── TodoItemRepository.cs
│       │   │   ├── RepositoryManager.cs
│       │   │   └── UnitOfWork.cs
│       │   └── Migrations/
│       │       ├── 20240114000000_InitialCreate.cs
│       │       ├── 20240114000000_InitialCreate.Designer.cs
│       │       └── RepositoryDbContextModelSnapshot.cs
│       ├── Extensions/
│       │   └── ServiceCollectionExtensions.cs
│       └── Common/
│           └── DatabaseOptions.cs
│
├── 2_Application/                   # Application layer
│   ├── Services.Abstractions/
│   │   ├── Starter.Services.Abstractions.csproj
│   │   ├── ITodoListService.cs
│   │   ├── ITodoItemService.cs
│   │   ├── INotificationService.cs
│   │   └── IServiceManager.cs
│   └── Services/
│       ├── Starter.Services.csproj
│       ├── TodoListService.cs
│       ├── TodoItemService.cs
│       ├── NotificationService.cs
│       ├── ServiceManager.cs
│       ├── Extensions/
│       │   ├── ServiceCollectionExtensions.cs
│       │   └── MappingConfiguration.cs
│       └── AssemblyReference.cs
│
├── 3_Run/                           # Runtime layer
│   ├── Web/
│   │   ├── Starter.Web.csproj
│   │   ├── Program.cs               # Main entry point
│   │   ├── appsettings.json         # Configuration
│   │   ├── appsettings.Development.json
│   │   ├── Dockerfile              # Container definition
│   │   ├── Controllers/
│   │   │   ├── TodoListsController.cs
│   │   │   └── TodoItemsController.cs
│   │   ├── Middleware/
│   │   │   └── ExceptionHandlingMiddleware.cs
│   │   └── Extensions/
│   │       └── ServiceCollectionExtensions.cs
│   ├── ServiceBus/
│   │   ├── Starter.ServiceBus.csproj
│   │   ├── Program.cs               # NServiceBus endpoint
│   │   ├── appsettings.json
│   │   ├── Handlers/
│   │   │   └── CreateTodoItemHandler.cs
│   │   └── AssemblyReference.cs
│   └── Docker/
│       ├── docker-compose.yml       # Local development setup
│       └── README.md
│
├── Tests/                           # Test layer
│   ├── UnitTests/
│   │   ├── Starter.UnitTests.csproj
│   │   └── Services/
│   │       └── TodoItemServiceTests.cs
│   ├── AcceptanceTests/
│   │   ├── Starter.AcceptanceTests.csproj
│   │   ├── TestFixture.cs           # WebApplicationFactory setup
│   │   └── TodoApiTests.cs          # API integration tests
│   └── ShouldlyFluentExtensions.cs
│
└── README.md                        # Backend-specific documentation
```

## Frontend Directory Structure

```
Frontend/
├── package.json                     # NPM dependencies and scripts
├── tsconfig.json                    # TypeScript configuration
├── tsconfig.node.json               # TypeScript config for Vite
├── vite.config.ts                   # Vite build configuration
├── eslint.config.js                 # ESLint configuration
├── .prettierrc                       # Prettier formatting
├── .gitignore                        # Git ignore patterns
├── index.html                        # HTML entry point
├── README.md                         # Frontend documentation
│
├── src/
│   ├── main.tsx                     # React entry point
│   ├── App.tsx                      # Root component
│   ├── index.css                    # Global styles
│   │
│   ├── api/
│   │   ├── client.ts                # Axios instance
│   │   └── todoApi.ts               # API service
│   │
│   ├── store/
│   │   └── todoStore.ts             # Zustand store
│   │
│   └── features/
│       ├── todoLists/
│       │   └── TodoListsView.tsx    # List management component
│       └── todoItems/
│           └── TodoItemsView.tsx    # Item management component
│
└── public/                          # Static assets
```

## Deploy Directory Structure

```
Deploy/
├── main.tf                          # Provider configuration
├── variables.tf                     # Input variables
├── outputs.tf                       # Output values
├── network.tf                       # VPC, subnets, routing
├── security_groups.tf               # Security group definitions
├── load_balancer.tf                 # ALB configuration
├── database.tf                      # RDS PostgreSQL
├── ecs.tf                           # ECS cluster, service, auto-scaling
├── README.md                        # Terraform documentation
│
├── environments/
│   ├── dev/
│   │   └── terraform.tfvars         # Development configuration
│   ├── uat/
│   │   └── terraform.tfvars         # UAT configuration
│   └── prod/
│       └── terraform.tfvars         # Production configuration
│
└── modules/                         # Reusable Terraform modules (future)
```

## Key Configuration Files

### Backend Configuration
| File | Location | Purpose |
|------|----------|---------|
| `appsettings.json` | `Backend/3_Run/Web/` | API settings, database connection |
| `Directory.Build.props` | Root | Global MSBuild settings |
| `Directory.Packages.props` | Root | Central NuGet package versions |
| `Starter.sln` | Root | Visual Studio solution |

### Frontend Configuration
| File | Location | Purpose |
|------|----------|---------|
| `package.json` | `Frontend/` | Dependencies, npm scripts |
| `vite.config.ts` | `Frontend/` | Build configuration |
| `tsconfig.json` | `Frontend/` | TypeScript settings |
| `eslint.config.js` | `Frontend/` | Linting rules |
| `.prettierrc` | `Frontend/` | Code formatting |

### Infrastructure Configuration
| File | Location | Purpose |
|------|----------|---------|
| `docker-compose.yml` | `Backend/3_Run/Docker/` | Local development |
| `Dockerfile` | `Backend/3_Run/Web/` | Container image |
| `terraform.tfvars` | `Deploy/environments/*/` | Environment variables |

## Documentation Files

| Document | Location | Content |
|----------|----------|---------|
| `README.md` | Root | Project overview, quick start |
| `GETTING_STARTED.md` | Root | 5-minute quick start guide |
| `TEMPLATE_SUMMARY.md` | Root | Implementation summary |
| `DELIVERABLES_CHECKLIST.md` | Root | Complete feature checklist |
| `Backend/README.md` | Backend | Backend documentation |
| `Frontend/README.md` | Frontend | Frontend documentation |
| `Deploy/README.md` | Deploy | Infrastructure documentation |
| `Rename-Template.ps1` | Root | Customization script |

## Critical Files for Development

### Backend Development
1. **Program.cs** - Entry point and DI configuration
2. **Domain Entities** - `Backend/0_Core/Domain/Entities/`
3. **Services** - `Backend/2_Application/Services/`
4. **Controllers** - `Backend/3_Run/Web/Controllers/`
5. **Migrations** - `Backend/1_Infrastructure/Infrastructure/Database/Migrations/`

### Frontend Development
1. **App.tsx** - Root component
2. **Components** - `Frontend/src/features/`
3. **API Client** - `Frontend/src/api/todoApi.ts`
4. **State Store** - `Frontend/src/store/`

### Infrastructure
1. **docker-compose.yml** - Local development setup
2. **main.tf** - Terraform provider configuration
3. **terraform.tfvars** - Environment-specific settings

## File Statistics

```
Total Directories: ~45
Total Files: ~150+
Source Files (.cs): ~40+
Test Files: ~8
React Components (.tsx): ~4
API/Store Files (.ts/.tsx): ~5
Configuration Files: ~20
Documentation Files: ~8
Terraform Files (.tf): ~12
Docker Files: ~2
```

## File Organization By Purpose

### Domain Layer Files
- Entities: `Backend/0_Core/Domain/Entities/`
- Repositories: `Backend/0_Core/Domain/Repositories/`
- Exceptions: `Backend/0_Core/Domain/Exceptions/`
- Messages: `Backend/0_Core/Messaging/`

### Data Access Files
- DbContext: `Backend/1_Infrastructure/Infrastructure/Database/`
- Repositories: `Backend/1_Infrastructure/Infrastructure/Database/Repositories/`
- Configurations: `Backend/1_Infrastructure/Infrastructure/Database/Configurations/`
- Migrations: `Backend/1_Infrastructure/Infrastructure/Database/Migrations/`

### Business Logic Files
- Services: `Backend/2_Application/Services/`
- Service Interfaces: `Backend/2_Application/Services.Abstractions/`

### API Endpoints
- Controllers: `Backend/3_Run/Web/Controllers/`

### Frontend UI
- Components: `Frontend/src/features/*/`
- API Client: `Frontend/src/api/`
- State: `Frontend/src/store/`

### Testing
- Unit Tests: `Backend/Tests/UnitTests/`
- Acceptance Tests: `Backend/Tests/AcceptanceTests/`

### Infrastructure
- AWS: `Deploy/*.tf`
- Docker: `Backend/3_Run/Docker/`

## Build Artifacts (Generated)

```
bin/                    # Compiled assemblies
obj/                    # Intermediate build files
dist/                   # Frontend production build
node_modules/          # NPM packages
.terraform/            # Terraform state and modules
```

## How to Navigate

### For Backend Development
1. Start with `Backend/README.md`
2. Examine domain entities: `Backend/0_Core/Domain/Entities/`
3. Review services: `Backend/2_Application/Services/`
4. Check controllers: `Backend/3_Run/Web/Controllers/`

### For Frontend Development
1. Start with `Frontend/README.md`
2. Review components: `Frontend/src/features/`
3. Check API integration: `Frontend/src/api/todoApi.ts`
4. Examine state: `Frontend/src/store/`

### For Deployment
1. Start with `Deploy/README.md`
2. Review Terraform files: `Deploy/*.tf`
3. Check environment configs: `Deploy/environments/*/`
4. Review Docker setup: `Backend/3_Run/Docker/`

### For Testing
1. Review unit tests: `Backend/Tests/UnitTests/`
2. Check acceptance tests: `Backend/Tests/AcceptanceTests/`
3. Run: `dotnet test`

## Quick Reference

### Essential Commands

```bash
# Build
dotnet build Starter.sln

# Test
dotnet test Starter.sln

# Database Migration
dotnet ef migrations add <Name> --project Backend/1_Infrastructure/Infrastructure --startup-project Backend/3_Run/Web

# Run API
cd Backend/3_Run/Web && dotnet run

# Run Frontend
cd Frontend && npm run dev

# Docker
docker compose -f Backend/3_Run/Docker/docker-compose.yml up

# Terraform
cd Deploy && terraform apply -var-file=environments/dev/terraform.tfvars

# Rename Template
.\Rename-Template.ps1 -NewProjectName "MyApp" -NewNamespace "MyCompany.MyApp"
```

## Next Steps

1. **Understand Architecture** - Read [Backend/README.md](Backend/README.md)
2. **Quick Start** - Follow [GETTING_STARTED.md](GETTING_STARTED.md)
3. **Verify Setup** - Run tests: `dotnet test`
4. **Customize** - Run `Rename-Template.ps1`
5. **Develop** - Add your business logic
6. **Deploy** - Use Terraform in `Deploy/`

---

**Template Navigation Complete!** 🎉

Use this index to quickly locate files and understand the project structure.
