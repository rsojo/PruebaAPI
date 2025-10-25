# PruebaAPI - API REST con .NET 8, Docker y PostgreSQL

API REST completa desarrollada con .NET 8, contenerizada con Docker y conectada a PostgreSQL.

## 🚀 Características

- **Framework**: .NET 8 Web API
- **Base de datos**: PostgreSQL 16
- **ORM**: Entity Framework Core con Npgsql
- **Arquitectura**: Patrón Repository
- **Contenerización**: Docker & Docker Compose
- **Documentación**: Swagger/OpenAPI
- **CRUD completo**: Operaciones para gestión de productos
- **Configuración separada**: Entity Type Configurations en archivos independientes

## 📋 Prerequisitos

- Docker Desktop instalado
- Docker Compose
- (Opcional) .NET 8 SDK para desarrollo local

## 🏗️ Estructura del Proyecto

```
PruebaAPI/
├── PruebaAPI/
│   ├── Controllers/
│   │   └── ProductsController.cs      # Controlador REST con CRUD
│   ├── Data/
│   │   ├── Configurations/
│   │   │   └── ProductConfiguration.cs # Configuración de entidad Product
│   │   └── AppDbContext.cs            # Contexto de Entity Framework
│   ├── Models/
│   │   └── Product.cs                 # Modelo de datos
│   ├── Repositories/
│   │   ├── IRepository.cs             # Interfaz genérica de repositorio
│   │   ├── Repository.cs              # Implementación genérica de repositorio
│   │   ├── IProductRepository.cs      # Interfaz específica de productos
│   │   └── ProductRepository.cs       # Implementación de repositorio de productos
│   ├── Migrations/                    # Migraciones de EF Core
│   ├── Program.cs                     # Configuración de la aplicación
│   └── appsettings.json              # Configuración (connection strings)
├── Dockerfile                         # Imagen Docker de la API
├── docker-compose.yml                 # Orquestación de contenedores
└── README.md
```

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

- La API usa **Patrón Repository** para la capa de acceso a datos
- Las configuraciones de entidades están separadas en archivos `IEntityTypeConfiguration`
- Entity Framework Core con migraciones aplicadas automáticamente al inicio
- Repositorio genérico (`IRepository<T>`) con métodos comunes reutilizables
- Repositorio específico (`IProductRepository`) con métodos especializados de negocio
- Inyección de dependencias configurada en `Program.cs`
- El contenedor de la API espera a que PostgreSQL esté healthy antes de iniciar
- Los datos de PostgreSQL se persisten en un volumen Docker

## 🚀 Próximos Pasos

- [ ] Agregar autenticación JWT
- [ ] Implementar paginación en los endpoints
- [ ] Agregar validaciones con FluentValidation
- [ ] Configurar logs con Serilog
- [ ] Agregar tests unitarios y de integración
- [ ] Implementar CI/CD con GitHub Actions

## 📄 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.
