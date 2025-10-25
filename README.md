# PruebaAPI - API REST con .NET 8, Docker, PostgreSQL y Clean Architecture

API REST completa desarrollada con .NET 8, implementando Clean Architecture, contenerizada con Docker y conectada a PostgreSQL.

## 🚀 Características

- **Framework**: .NET 8 Web API
- **Arquitectura**: Clean Architecture (4 capas)
- **Patrones**: Repository, Unit of Work, Service Layer
- **Base de datos**: PostgreSQL 16
- **ORM**: Entity Framework Core con Npgsql
- **DTOs**: Separación entre modelos de dominio y API
- **Contenerización**: Docker & Docker Compose
- **Documentación**: Swagger/OpenAPI
- **CRUD completo**: Operaciones para gestión de productos

## 📋 Prerequisitos

- Docker Desktop instalado
- Docker Compose
- (Opcional) .NET 8 SDK para desarrollo local

## 🏗️ Estructura del Proyecto (Clean Architecture)

```
PruebaAPI/
├── PruebaAPI/
│   ├── Domain/                          # ⭐ Capa de Dominio (Núcleo)
│   │   ├── Entities/
│   │   │   └── Product.cs              # Entidades de negocio
│   │   └── Interfaces/
│   │       ├── IRepository.cs          # Contratos genéricos
│   │       ├── IProductRepository.cs   # Contratos específicos
│   │       └── IUnitOfWork.cs          # Patrón Unit of Work
│   │
│   ├── Application/                     # ⭐ Capa de Aplicación (Casos de Uso)
│   │   ├── DTOs/
│   │   │   ├── ProductDto.cs          # DTO de lectura
│   │   │   ├── CreateProductDto.cs    # DTO de creación
│   │   │   └── UpdateProductDto.cs    # DTO de actualización
│   │   ├── Interfaces/
│   │   │   └── IProductService.cs     # Interfaz de servicio
│   │   └── Services/
│   │       └── ProductService.cs      # Lógica de aplicación
│   │
│   ├── Infrastructure/                  # ⭐ Capa de Infraestructura (Implementación)
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs        # DbContext de EF Core
│   │   │   └── Configurations/
│   │   │       └── ProductConfiguration.cs # Config de entidades
│   │   └── Repositories/
│   │       ├── Repository.cs          # Repositorio genérico
│   │       ├── ProductRepository.cs   # Repositorio específico
│   │       └── UnitOfWork.cs          # Implementación UoW
│   │
│   ├── Presentation/                    # ⭐ Capa de Presentación (API)
│   │   └── Controllers/
│   │       └── ProductsController.cs  # Controlador REST
│   │
│   ├── Migrations/                     # Migraciones de EF Core
│   ├── Program.cs                      # Configuración y DI
│   └── appsettings.json               # Configuración
│
├── Dockerfile                          # Imagen Docker de la API
├── docker-compose.yml                  # Orquestación de contenedores
├── README.md                           # Este archivo
└── CLEAN_ARCHITECTURE.md              # Documentación detallada de la arquitectura
```

### 📐 Dependencias entre Capas

```
Presentation  →  Application  →  Domain  ←  Infrastructure
    (API)         (Services)    (Entities)    (Data Access)
```

- **Domain**: No depende de nadie (núcleo puro)
- **Application**: Depende solo de Domain
- **Infrastructure**: Implementa interfaces de Domain
- **Presentation**: Depende de Application

## 🐳 Ejecutar con Docker

### 1. Clonar el repositorio

```bash
git clone <tu-repo>
cd PruebaAPI
```

### 2. Iniciar los contenedores

```bash
docker-compose up --build
```

Este comando:
- Construye la imagen Docker de la API
- Inicia el contenedor de PostgreSQL
- Inicia el contenedor de la API
- Configura la red entre contenedores

### 3. Acceder a la aplicación

- **API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger
- **PostgreSQL**: localhost:5432
  - Usuario: `postgres`
  - Contraseña: `postgres123`
  - Base de datos: `pruebaapi`

## 🔧 Configuración de la Base de Datos

### Crear las migraciones

```bash
cd PruebaAPI
dotnet ef migrations add InitialCreate
```

### Aplicar migraciones

```bash
dotnet ef database update
```

O desde Docker:

```bash
docker-compose exec api dotnet ef database update
```

## 📡 Endpoints de la API

### Products

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/products` | Obtener todos los productos |
| GET | `/api/products/{id}` | Obtener un producto por ID |
| GET | `/api/products/search?name={name}` | Buscar productos por nombre |
| GET | `/api/products/in-stock` | Obtener productos en stock |
| GET | `/api/products/price-range?minPrice={min}&maxPrice={max}` | Obtener productos por rango de precio |
| POST | `/api/products` | Crear un nuevo producto |
| PUT | `/api/products/{id}` | Actualizar un producto |
| DELETE | `/api/products/{id}` | Eliminar un producto |

### Ejemplo de Producto (JSON)

```json
{
  "name": "Laptop",
  "description": "High performance laptop",
  "price": 1299.99,
  "stock": 10
}
```

## 🧪 Probar la API

### Con cURL

```bash
# Obtener todos los productos
curl http://localhost:8080/api/products

# Obtener un producto específico
curl http://localhost:8080/api/products/1

# Buscar productos por nombre
curl "http://localhost:8080/api/products/search?name=laptop"

# Obtener productos en stock
curl http://localhost:8080/api/products/in-stock

# Obtener productos por rango de precio
curl "http://localhost:8080/api/products/price-range?minPrice=20&maxPrice=100"

# Crear un producto
curl -X POST http://localhost:8080/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Monitor",
    "description": "27 inch 4K monitor",
    "price": 399.99,
    "stock": 15
  }'

# Actualizar un producto
curl -X PUT http://localhost:8080/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "name": "Laptop Pro",
    "description": "Updated description",
    "price": 1499.99,
    "stock": 8,
    "createdAt": "2025-10-25T00:00:00Z"
  }'

# Eliminar un producto
curl -X DELETE http://localhost:8080/api/products/1
```

### Con Swagger UI

1. Abre http://localhost:8080/swagger
2. Explora los endpoints disponibles
3. Haz clic en "Try it out" para probar las peticiones

## 🛠️ Desarrollo Local (sin Docker)

### 1. Instalar dependencias

```bash
cd PruebaAPI
dotnet restore
```

### 2. Configurar la base de datos

Asegúrate de tener PostgreSQL corriendo localmente y actualiza `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=pruebaapi;Username=postgres;Password=tu_password"
  }
}
```

### 3. Ejecutar migraciones

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Ejecutar la aplicación

```bash
dotnet run
```

La API estará disponible en http://localhost:5000

## 🐳 Comandos Docker Útiles

```bash
# Ver logs de los contenedores
docker-compose logs -f

# Ver logs solo de la API
docker-compose logs -f api

# Ver logs solo de PostgreSQL
docker-compose logs -f postgres

# Detener los contenedores
docker-compose down

# Detener y eliminar volúmenes (datos de BD)
docker-compose down -v

# Reiniciar un servicio específico
docker-compose restart api

# Ejecutar comandos en el contenedor
docker-compose exec api bash
docker-compose exec postgres psql -U postgres -d pruebaapi
```

## 📦 Variables de Entorno

### API (en docker-compose.yml)

- `ASPNETCORE_ENVIRONMENT`: Entorno de ejecución (Development/Production)
- `ConnectionStrings__DefaultConnection`: String de conexión a PostgreSQL

### PostgreSQL

- `POSTGRES_DB`: Nombre de la base de datos
- `POSTGRES_USER`: Usuario de PostgreSQL
- `POSTGRES_PASSWORD`: Contraseña de PostgreSQL

## 🔍 Solución de Problemas

### La API no puede conectarse a PostgreSQL

- Verifica que el contenedor de PostgreSQL esté corriendo: `docker-compose ps`
- Revisa los logs: `docker-compose logs postgres`
- Asegúrate de que el healthcheck esté pasando

### Errores de migración

```bash
# Eliminar todas las migraciones
rm -rf PruebaAPI/Migrations

# Recrear las migraciones
cd PruebaAPI
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Reconstruir contenedores

```bash
docker-compose down -v
docker-compose build --no-cache
docker-compose up
```

## 📝 Notas Técnicas

### Arquitectura
- Implementa **Clean Architecture** con 4 capas bien definidas
- **Domain Layer**: Entidades e interfaces de negocio (sin dependencias)
- **Application Layer**: Casos de uso, servicios y DTOs
- **Infrastructure Layer**: Acceso a datos con EF Core, repositorios
- **Presentation Layer**: Controladores REST API

### Patrones de Diseño
- **Repository Pattern**: Abstracción del acceso a datos
- **Unit of Work Pattern**: Gestión de transacciones y persistencia
- **Service Layer Pattern**: Lógica de aplicación en servicios
- **DTO Pattern**: Separación entre dominio y contratos de API

### Tecnologías
- Entity Framework Core con migraciones aplicadas automáticamente
- Inyección de dependencias nativa de .NET
- Repositorio genérico reutilizable
- DTOs para prevenir over-posting y mejorar seguridad
- El contenedor de la API espera a que PostgreSQL esté healthy antes de iniciar
- Los datos de PostgreSQL se persisten en un volumen Docker

### Ventajas
- ✅ Alta testabilidad (interfaces mockeables)
- ✅ Bajo acoplamiento entre capas
- ✅ Alto cohesión dentro de cada capa
- ✅ Fácil de extender y mantener
- ✅ Independiente de frameworks y base de datos

## 🚀 Próximos Pasos

- [ ] Agregar AutoMapper para mapeo automático de DTOs
- [ ] Implementar FluentValidation para validación de DTOs
- [ ] Agregar pruebas unitarias y de integración
- [ ] Implementar autenticación JWT
- [ ] Agregar paginación genérica en repositorios
- [ ] Implementar CQRS con MediatR
- [ ] Agregar logging con Serilog
- [ ] Implementar caché con Redis
- [ ] Agregar health checks
- [ ] Implementar CI/CD con GitHub Actions

## 📚 Documentación Adicional

- **[CLEAN_ARCHITECTURE.md](./CLEAN_ARCHITECTURE.md)** - Guía completa de Clean Architecture implementada
- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Documentación del patrón Repository (versión anterior)

## 🎓 Recursos de Aprendizaje

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Clean Architecture Template - Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture)
- [Microsoft - Clean Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture)

## 📄 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.
