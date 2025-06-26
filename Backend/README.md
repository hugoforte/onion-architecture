# Payments API - Onion Architecture Implementation

This project demonstrates the implementation of **Onion Architecture** (also known as Clean Architecture or Ports and Adapters) in ASP.NET Core. The architecture is structured as concentric circles with dependencies flowing inward toward the core.

## Architecture Overview

The solution follows the Onion Architecture pattern with four main layers:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│                    (Payments.Web)                          │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                     │
│                    (Payments.Persistence)                  │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                    Service Layer                            │
│                    (Payments.Services)                     │
│                    (Payments.Services.Abstractions)        │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                    Domain Layer                             │
│                    (Payments.Domain)                       │
│                    (Payments.Contracts)                    │
└─────────────────────────────────────────────────────────────┘
```

## Project Structure

- **Payments.Web** - ASP.NET Core Web API (Presentation Layer)
- **Payments.Presentation** - Controllers and API endpoints
- **Payments.Services** - Business logic implementation
- **Payments.Services.Abstractions** - Service interfaces
- **Payments.Domain** - Core business entities and interfaces
- **Payments.Persistence** - Data access and database implementation
- **Payments.Contracts** - Data Transfer Objects (DTOs)

## Key Principles

1. **Dependency Inversion**: All dependencies flow inward toward the Domain layer
2. **Separation of Concerns**: Each layer has a specific responsibility
3. **Testability**: High testability through dependency injection and interfaces
4. **Maintainability**: Clear boundaries between layers make the codebase maintainable

## Getting Started

### Prerequisites
- .NET 9.0
- Docker and Docker Compose
- PostgreSQL (via Docker)

### Running the Application

1. **Using Docker Compose** (Recommended):
   ```bash
   docker-compose up
   ```

2. **Manual Setup**:
   - Update connection string in `appsettings.json`
   - Run Entity Framework migrations
   - Start the application

### API Documentation

Once running, access the Swagger UI at:
```
https://localhost:5001/swagger
```

## Features

- RESTful API for managing Billers
- Entity Framework Core with PostgreSQL
- Global exception handling
- Automatic database migrations
- Docker containerization
- Swagger API documentation

## Architecture Benefits

- **Testability**: Easy to unit test business logic in isolation
- **Flexibility**: Can swap implementations without affecting other layers
- **Maintainability**: Clear separation of concerns
- **Scalability**: Well-defined boundaries support team development 