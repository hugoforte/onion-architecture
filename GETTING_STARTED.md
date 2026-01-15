# Getting Started with Todo App Template

A quick guide to get the Todo App template up and running.

## Prerequisites

- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js 22+** - [Download](https://nodejs.org/en/download)
- **PostgreSQL 16** (or use Docker) - [Download](https://www.postgresql.org/download/)
- **Docker & Docker Compose** (optional) - [Download](https://www.docker.com/products/docker-desktop)
- **Git** - [Download](https://git-scm.com/download)

## Quick Start (5 minutes)

### Option 1: With Docker Compose (Recommended)

```powershell
# Navigate to Docker directory
cd Backend\3_Run\Docker

# Start all services
docker compose up -d

# Wait for services to be healthy (~30 seconds)
docker compose ps

# Frontend
cd Frontend
npm install
npm run dev
# Access at http://localhost:5173
```

### Option 2: Manual Local Setup

```powershell
# 1. Start PostgreSQL
# (Skip if you already have PostgreSQL running or use Docker)
docker run --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16-alpine

# 2. Build and run backend
cd Backend\3_Run\Web
dotnet restore
dotnet build
dotnet run

# API will be available at http://localhost:5003
# Swagger UI at http://localhost:5003/swagger

# 3. In a new terminal, run frontend
cd Frontend
npm install
npm run dev

# Frontend will be available at http://localhost:5173
```

## Verify Everything Works

### 1. Test Backend

```powershell
# Run all tests (should pass)
dotnet test Starter.sln

# Or just unit tests
dotnet test Backend/Tests/UnitTests/
```

### 2. Test API

```powershell
# Create a todo list
curl -X POST http://localhost:5003/api/todo-lists `
  -H "Content-Type: application/json" `
  -d '{\"name\":\"My List\",\"description\":\"Test\"}'

# Get all lists
curl http://localhost:5003/api/todo-lists
```

### 3. Test Frontend

Open browser to `http://localhost:5173` and:
- Create a todo list
- Add items to the list
- Mark items as complete
- Delete items

## Customizing the Template

### Rename Project

To use this template for your own project, run the rename script:

```powershell
# From template root directory
.\Rename-Template.ps1 -NewProjectName "MyApp" -NewNamespace "MyCompany.MyApp"
```

This will:
- Rename all projects and folders
- Update namespaces throughout C# code
- Update configuration files
- Update Docker/Terraform configs

### After Renaming

```powershell
# Clean build
dotnet clean
dotnet build

# Run tests
dotnet test

# Test locally
cd Backend\3_Run\Web
dotnet run
```

## Project Structure

```
Template/
├── Backend/              # ASP.NET Core API
│   ├── 0_Core/          # Domain layer
│   ├── 1_Infrastructure/# Data access layer
│   ├── 2_Application/   # Business logic layer
│   ├── 3_Run/           # Runtime (Web, ServiceBus)
│   └── Tests/           # Unit and acceptance tests
├── Frontend/            # React app
├── Deploy/              # Terraform for AWS
└── README.md            # Full documentation
```

## Development Workflow

### Adding a New Entity

1. Create entity in `Backend/0_Core/Domain/Entities/`
2. Create DTO in `Backend/0_Core/Contracts/`
3. Add repository in `Backend/0_Core/Domain/Repositories/`
4. Implement repository in `Backend/1_Infrastructure/Database/Repositories/`
5. Create service in `Backend/2_Application/Services/`
6. Add controller in `Backend/3_Run/Web/Controllers/`
7. Create migration: 
   ```powershell
   dotnet ef migrations add AddMyEntity `
     --project Backend/1_Infrastructure/Infrastructure `
     --startup-project Backend/3_Run/Web
   ```

### Adding a New API Endpoint

1. Add method to service in `Backend/2_Application/Services/`
2. Add endpoint to controller in `Backend/3_Run/Web/Controllers/`
3. Add acceptance test in `Backend/Tests/AcceptanceTests/`
4. Run tests to verify

### Adding Frontend Component

1. Create component in `Frontend/src/features/`
2. Add API calls in `Frontend/src/api/`
3. Import in parent component
4. Test in browser at `http://localhost:5173`

## Common Commands

### Backend

```powershell
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests
dotnet test

# Run API
cd Backend/3_Run/Web
dotnet run

# Database migration
dotnet ef migrations add <MigrationName> `
  --project Backend/1_Infrastructure/Infrastructure `
  --startup-project Backend/3_Run/Web

# Apply migration
dotnet ef database update `
  --project Backend/1_Infrastructure/Infrastructure `
  --startup-project Backend/3_Run/Web
```

### Frontend

```powershell
cd Frontend

# Install dependencies
npm install

# Development server (http://localhost:5173)
npm run dev

# Build for production
npm run build

# Linting
npm run lint

# Format code
npm run format
```

### Docker

```powershell
# Start services
docker compose -f Backend/3_Run/Docker/docker-compose.yml up -d

# Stop services
docker compose -f Backend/3_Run/Docker/docker-compose.yml down

# View logs
docker compose -f Backend/3_Run/Docker/docker-compose.yml logs -f

# Rebuild images
docker compose -f Backend/3_Run/Docker/docker-compose.yml up --build
```

## Deployment

### Docker Build

```bash
docker build -t my-app:latest Backend/3_Run/Web
docker run -p 5003:5003 my-app:latest
```

### AWS Deployment

```bash
cd Deploy
terraform init
terraform plan -var-file=environments/dev/terraform.tfvars
terraform apply -var-file=environments/dev/terraform.tfvars
```

See [Deploy/README.md](Deploy/README.md) for detailed instructions.

## Troubleshooting

### Port Already in Use

```powershell
# Find process using port
netstat -ano | findstr :5003

# Kill process (replace PID)
taskkill /PID <PID> /F

# Or use different port in appsettings.json
```

### Database Connection Error

```powershell
# Check PostgreSQL is running
docker ps

# If not running with Docker:
docker run --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16-alpine

# Check connection string in appsettings.json
```

### NPM Dependencies Issue

```powershell
cd Frontend
npm cache clean --force
rm -r node_modules package-lock.json
npm install
```

## Resources

- [Backend Documentation](Backend/README.md)
- [Frontend Documentation](Frontend/README.md)
- [Infrastructure Documentation](Deploy/README.md)
- [ASP.NET Core Docs](https://docs.microsoft.com/en-us/aspnet/core)
- [React Docs](https://react.dev)
- [Terraform Docs](https://www.terraform.io/docs)

## Next Steps

1. ✅ Clone/copy this template
2. ✅ Run Quick Start above
3. ✅ Verify tests pass
4. ✅ Customize with `Rename-Template.ps1`
5. → Add your domain entities and business logic
6. → Build your features
7. → Deploy to production

## Support

For issues or questions:
- Check [Backend/README.md](Backend/README.md)
- Check [Frontend/README.md](Frontend/README.md)
- Check [Deploy/README.md](Deploy/README.md)
- Review example code in existing services and components

Happy coding! 🚀
