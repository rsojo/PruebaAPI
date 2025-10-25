# PruebaAPI - Car Brands Management API

RESTful API for managing car brands built with .NET 8, PostgreSQL, and Docker. Implements Clean Architecture with modern development patterns.

## Features

- **Framework**: .NET 8 Web API
- **Architecture**: Clean Architecture (4 layers)
- **Design Patterns**: Repository, Unit of Work, Service Layer
- **Database**: PostgreSQL 16 with Entity Framework Core
- **Containerization**: Docker & Docker Compose
- **Documentation**: Swagger/OpenAPI
- **Testing**: xUnit, FluentAssertions, Moq (45 tests, 47.5% coverage)
- **CRUD Operations**: Complete car brand management

## Prerequisites

- Docker Desktop
- Docker Compose
- (Optional) .NET 8 SDK for local development

## Quick Start

```bash
# Clone the repository
git clone <repository-url>
cd PruebaAPI

# Start containers
docker-compose up -d

# API available at: http://localhost:8080
# Swagger UI: http://localhost:8080/swagger
```

## Project Structure

```
PruebaAPI/
├── Domain/                    # Core business entities and interfaces
│   ├── Entities/             # MarcaAuto entity
│   └── Interfaces/           # Repository contracts
├── Application/              # Business logic and use cases
│   ├── DTOs/                # Data Transfer Objects
│   ├── Interfaces/          # Service contracts
│   └── Services/            # Service implementations
├── Infrastructure/           # Data access and external concerns
│   ├── Persistence/         # DbContext and configurations
│   ├── Data/                # Database seeder
│   └── Repositories/        # Repository implementations
├── Presentation/            # API endpoints
│   └── Controllers/         # REST controllers
└── PruebaAPI.Tests/         # Unit and integration tests
```

## API Endpoints

**Base URL**: `http://localhost:8080/api/MarcasAutos`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/` | Get all car brands |
| `GET` | `/{id}` | Get brand by ID |
| `GET` | `/buscar?nombre={name}` | Search brands by name |
| `GET` | `/activas` | Get active brands |
| `GET` | `/pais/{country}` | Filter by country of origin |
| `GET` | `/anio-fundacion?añoInicio={start}&añoFin={end}` | Filter by foundation year range |
| `POST` | `/` | Create new brand |
| `PUT` | `/{id}` | Update existing brand |
| `DELETE` | `/{id}` | Delete brand |

## Data Model

```json
{
  "id": 1,
  "nombre": "Toyota",
  "paisOrigen": "Japón",
  "añoFundacion": 1937,
  "sitioWeb": "https://www.toyota.com",
  "esActiva": true,
  "fechaCreacion": "2025-10-25T00:00:00Z"
}
```

## Examples

### Get all brands
```bash
curl http://localhost:8080/api/MarcasAutos
```

### Search by name
```bash
curl "http://localhost:8080/api/MarcasAutos/buscar?nombre=toyota"
```

### Create new brand
```bash
curl -X POST http://localhost:8080/api/MarcasAutos \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Volkswagen",
    "paisOrigen": "Alemania",
    "añoFundacion": 1937,
    "sitioWeb": "https://www.volkswagen.com",
    "esActiva": true
  }'
```

### Update brand
```bash
curl -X PUT http://localhost:8080/api/MarcasAutos/1 \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "nombre": "Toyota Motor Corporation",
    "paisOrigen": "Japón",
    "añoFundacion": 1937,
    "sitioWeb": "https://www.toyota.com",
    "esActiva": true
  }'
```

### Delete brand
```bash
curl -X DELETE http://localhost:8080/api/MarcasAutos/1
```

## Local Development

### Without Docker

```bash
# Install dependencies
dotnet restore

# Update connection string in appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=pruebaapi;Username=postgres;Password=your_password"
  }
}

# Apply migrations
dotnet ef database update

# Run application
dotnet run

# API available at: http://localhost:5000
```

### Run Tests

```bash
# Execute all tests
dotnet test

# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport"
```

## Docker Commands

```bash
# View logs
docker-compose logs -f

# Restart services
docker-compose restart

# Stop and remove containers
docker-compose down

# Stop and remove containers with volumes
docker-compose down -v

# Rebuild and restart
docker-compose up -d --build
```

## Environment Variables

### API Container
- `ASPNETCORE_ENVIRONMENT`: Runtime environment (Development/Production)
- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string

### PostgreSQL Container
- `POSTGRES_DB`: Database name
- `POSTGRES_USER`: PostgreSQL user
- `POSTGRES_PASSWORD`: PostgreSQL password

## Architecture

### Clean Architecture Layers

```
Presentation  →  Application  →  Domain  ←  Infrastructure
    (API)        (Services)    (Entities)   (Data Access)
```

- **Domain**: Core business entities (no dependencies)
- **Application**: Business logic and DTOs
- **Infrastructure**: Data access implementations
- **Presentation**: REST API endpoints

### Design Patterns

- **Repository Pattern**: Data access abstraction
- **Unit of Work Pattern**: Transaction management
- **Service Layer Pattern**: Business logic orchestration
- **DTO Pattern**: API contract separation

## Testing

### Test Metrics

| Metric | Coverage | Details |
|--------|----------|---------|
| **Total Tests** | 45 | All passing |
| **Line Coverage** | 47.5% | 272 / 572 lines |
| **Branch Coverage** | 71.4% | 35 / 49 branches |
| **Method Coverage** | 84.2% | 64 / 76 methods |

### Coverage by Layer

| Layer | Coverage | Components |
|-------|----------|------------|
| **Domain** | 100% | Entities |
| **Application** | 100% | Services, DTOs |
| **Infrastructure** | 100% | Repositories |
| **Presentation** | 100% | Controllers |
| **Unit of Work** | 90.6% | Transaction management |
| **Framework** | 0% | Program.cs, EF Core, Migrations |

### Test Distribution

- Service Layer: 13 tests
- Controller Layer: 14 tests  
- Repository Layer: 9 tests
- Unit of Work: 9 tests

### Untested Components

Components intentionally not covered by unit tests:
- Application entry point (Program.cs)
- Entity Framework infrastructure (AppDbContext)
- Auto-generated migrations
- Development seed data (DbSeeder)

## Troubleshooting

### API not responding
```bash
docker ps
docker logs pruebaapi-api
```

### Database connection issues
```bash
docker logs pruebaapi-postgres
docker-compose restart
```

### Reset everything
```bash
docker-compose down -v
docker-compose up -d --build
```

## Additional Documentation

- [ARQUITECTURA.md](./ARQUITECTURA.md) - Detailed architecture documentation (Spanish)

## Technology Stack

- .NET 8
- Entity Framework Core 9.0
- PostgreSQL 16
- Docker & Docker Compose
- Swagger/OpenAPI
- xUnit, FluentAssertions, Moq

## License

This project is open source and available under the MIT License.
