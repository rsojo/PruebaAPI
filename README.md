# 🚗 API de Marcas de Autos# PruebaAPI - API REST con .NET 8, Docker, PostgreSQL y Clean Architecture



API RESTful para gestionar marcas de automóviles, construida con **.NET 8**, **PostgreSQL** y **Docker**. Implementa Clean Architecture con patrones modernos de desarrollo.API REST completa desarrollada con .NET 8, implementando Clean Architecture, contenerizada con Docker y conectada a PostgreSQL.



## 📋 Características## 🚀 Características



- ✅ CRUD completo de marcas de autos- **Framework**: .NET 8 Web API

- ✅ Búsqueda y filtros avanzados- **Arquitectura**: Clean Architecture (4 capas)

- ✅ Clean Architecture (4 capas)- **Patrones**: Repository, Unit of Work, Service Layer

- ✅ Repository Pattern + Unit of Work- **Base de datos**: PostgreSQL 16

- ✅ Entity Framework Core con configuraciones fluidas- **ORM**: Entity Framework Core con Npgsql

- ✅ Contenedorización con Docker- **DTOs**: Separación entre modelos de dominio y API

- ✅ Base de datos PostgreSQL- **Contenerización**: Docker & Docker Compose

- ✅ Swagger/OpenAPI integrado- **Documentación**: Swagger/OpenAPI

- ✅ Migraciones automáticas- **CRUD completo**: Operaciones para gestión de productos



## 🚀 Inicio Rápido## 📋 Prerequisitos



### Prerrequisitos- Docker Desktop instalado

- Docker Compose

- Docker & Docker Compose- (Opcional) .NET 8 SDK para desarrollo local

- .NET 8 SDK (para desarrollo local)

## 🏗️ Estructura del Proyecto (Clean Architecture)

### Levantar la aplicación

```

```bashPruebaAPI/

# Clonar el repositorio├── PruebaAPI/

git clone <repository-url>│   ├── Domain/                          # ⭐ Capa de Dominio (Núcleo)

cd PruebaAPI│   │   ├── Entities/

│   │   │   └── Product.cs              # Entidades de negocio

# Iniciar con Docker│   │   └── Interfaces/

docker-compose up -d│   │       ├── IRepository.cs          # Contratos genéricos

│   │       ├── IProductRepository.cs   # Contratos específicos

# La API estará disponible en:│   │       └── IUnitOfWork.cs          # Patrón Unit of Work

# http://localhost:8080│   │

# Swagger UI: http://localhost:8080/swagger│   ├── Application/                     # ⭐ Capa de Aplicación (Casos de Uso)

```│   │   ├── DTOs/

│   │   │   ├── ProductDto.cs          # DTO de lectura

### Detener la aplicación│   │   │   ├── CreateProductDto.cs    # DTO de creación

│   │   │   └── UpdateProductDto.cs    # DTO de actualización

```bash│   │   ├── Interfaces/

docker-compose down│   │   │   └── IProductService.cs     # Interfaz de servicio

```│   │   └── Services/

│   │       └── ProductService.cs      # Lógica de aplicación

## 📡 Endpoints de la API│   │

│   ├── Infrastructure/                  # ⭐ Capa de Infraestructura (Implementación)

### Base URL: `http://localhost:8080/api/MarcasAutos`│   │   ├── Persistence/

│   │   │   ├── AppDbContext.cs        # DbContext de EF Core

| Método | Endpoint | Descripción |│   │   │   └── Configurations/

|--------|----------|-------------|│   │   │       └── ProductConfiguration.cs # Config de entidades

| `GET` | `/` | Obtener todas las marcas |│   │   └── Repositories/

| `GET` | `/{id}` | Obtener marca por ID |│   │       ├── Repository.cs          # Repositorio genérico

| `GET` | `/buscar?nombre={nombre}` | Buscar marcas por nombre |│   │       ├── ProductRepository.cs   # Repositorio específico

| `GET` | `/activas` | Obtener marcas activas |│   │       └── UnitOfWork.cs          # Implementación UoW

| `GET` | `/pais/{pais}` | Filtrar por país de origen |│   │

| `GET` | `/anio-fundacion?añoInicio={año}&añoFin={año}` | Filtrar por rango de años |│   ├── Presentation/                    # ⭐ Capa de Presentación (API)

| `POST` | `/` | Crear nueva marca |│   │   └── Controllers/

| `PUT` | `/{id}` | Actualizar marca existente |│   │       └── ProductsController.cs  # Controlador REST

| `DELETE` | `/{id}` | Eliminar marca |│   │

│   ├── Migrations/                     # Migraciones de EF Core

## 📝 Ejemplos de Uso│   ├── Program.cs                      # Configuración y DI

│   └── appsettings.json               # Configuración

### Obtener todas las marcas│

├── Dockerfile                          # Imagen Docker de la API

```bash├── docker-compose.yml                  # Orquestación de contenedores

curl http://localhost:8080/api/MarcasAutos├── README.md                           # Este archivo

```└── CLEAN_ARCHITECTURE.md              # Documentación detallada de la arquitectura

```

### Buscar marcas por nombre

### 📐 Dependencias entre Capas

```bash

curl "http://localhost:8080/api/MarcasAutos/buscar?nombre=toyota"```

```Presentation  →  Application  →  Domain  ←  Infrastructure

    (API)         (Services)    (Entities)    (Data Access)

### Obtener marcas de un país```



```bash- **Domain**: No depende de nadie (núcleo puro)

curl http://localhost:8080/api/MarcasAutos/pais/Alemania- **Application**: Depende solo de Domain

```- **Infrastructure**: Implementa interfaces de Domain

- **Presentation**: Depende de Application

### Crear una nueva marca

## 🐳 Ejecutar con Docker

```bash

curl -X POST http://localhost:8080/api/MarcasAutos \### 1. Clonar el repositorio

  -H "Content-Type: application/json" \

  -d '{```bash

    "nombre": "Volkswagen",git clone <tu-repo>

    "paisOrigen": "Alemania",cd PruebaAPI

    "añoFundacion": 1937,```

    "sitioWeb": "https://www.volkswagen.com",

    "esActiva": true### 2. Iniciar los contenedores

  }'

``````bash

docker-compose up --build

### Actualizar una marca```



```bashEste comando:

curl -X PUT http://localhost:8080/api/MarcasAutos/1 \- Construye la imagen Docker de la API

  -H "Content-Type: application/json" \- Inicia el contenedor de PostgreSQL

  -d '{- Inicia el contenedor de la API

    "id": 1,- Configura la red entre contenedores

    "nombre": "Toyota Motor Corporation",

    "paisOrigen": "Japón",### 3. Acceder a la aplicación

    "añoFundacion": 1937,

    "sitioWeb": "https://www.toyota.com",- **API**: http://localhost:8080

    "esActiva": true- **Swagger UI**: http://localhost:8080/swagger

  }'- **PostgreSQL**: localhost:5432

```  - Usuario: `postgres`

  - Contraseña: `postgres123`

### Eliminar una marca  - Base de datos: `pruebaapi`



```bash## 🔧 Configuración de la Base de Datos

curl -X DELETE http://localhost:8080/api/MarcasAutos/1

```### Crear las migraciones



## 🗂️ Modelo de Datos```bash

cd PruebaAPI

### MarcaAutodotnet ef migrations add InitialCreate

```

```json

{### Aplicar migraciones

  "id": 1,

  "nombre": "Toyota",```bash

  "paisOrigen": "Japón",dotnet ef database update

  "añoFundacion": 1937,```

  "sitioWeb": "https://www.toyota.com",

  "esActiva": true,O desde Docker:

  "fechaCreacion": "2025-10-25T00:00:00Z"

}```bash

```docker-compose exec api dotnet ef database update

```

| Campo | Tipo | Descripción | Requerido |

|-------|------|-------------|-----------|## 📡 Endpoints de la API

| `id` | int | Identificador único (auto-generado) | No |

| `nombre` | string | Nombre de la marca (máx. 100 caracteres) | Sí |### Products

| `paisOrigen` | string | País de origen (máx. 100 caracteres) | No |

| `añoFundacion` | int | Año de fundación de la marca | Sí || Método | Endpoint | Descripción |

| `sitioWeb` | string | URL del sitio web oficial (máx. 200 caracteres) | No ||--------|----------|-------------|

| `esActiva` | bool | Indica si la marca está activa (default: true) | Sí || GET | `/api/products` | Obtener todos los productos |

| `fechaCreacion` | datetime | Fecha de registro en el sistema | No || GET | `/api/products/{id}` | Obtener un producto por ID |

| GET | `/api/products/search?name={name}` | Buscar productos por nombre |

## 🛠️ Desarrollo Local| GET | `/api/products/in-stock` | Obtener productos en stock |

| GET | `/api/products/price-range?minPrice={min}&maxPrice={max}` | Obtener productos por rango de precio |

### Requisitos| POST | `/api/products` | Crear un nuevo producto |

| PUT | `/api/products/{id}` | Actualizar un producto |

- .NET 8 SDK| DELETE | `/api/products/{id}` | Eliminar un producto |

- PostgreSQL (o usar Docker)

### Ejemplo de Producto (JSON)

### Configurar la base de datos

```json

Editar `appsettings.Development.json`:{

  "name": "Laptop",

```json  "description": "High performance laptop",

{  "price": 1299.99,

  "ConnectionStrings": {  "stock": 10

    "DefaultConnection": "Host=localhost;Port=5432;Database=pruebaapi;Username=postgres;Password=postgres"}

  }```

}

```## 🧪 Probar la API



### Ejecutar migraciones### Con cURL



```bash```bash

# Aplicar migraciones# Obtener todos los productos

dotnet ef database update --project PruebaAPI/PruebaAPI.csprojcurl http://localhost:8080/api/products



# Crear nueva migración# Obtener un producto específico

dotnet ef migrations add NombreMigracion --project PruebaAPI/PruebaAPI.csprojcurl http://localhost:8080/api/products/1

```

# Buscar productos por nombre

### Ejecutar la aplicacióncurl "http://localhost:8080/api/products/search?name=laptop"



```bash# Obtener productos en stock

cd PruebaAPIcurl http://localhost:8080/api/products/in-stock

dotnet run

```# Obtener productos por rango de precio

curl "http://localhost:8080/api/products/price-range?minPrice=20&maxPrice=100"

La API estará disponible en `http://localhost:5000`

# Crear un producto

### Ejecutar testscurl -X POST http://localhost:8080/api/products \

  -H "Content-Type: application/json" \

```bash  -d '{

dotnet test PruebaAPI.Tests/PruebaAPI.Tests.csproj    "name": "Monitor",

```    "description": "27 inch 4K monitor",

    "price": 399.99,

## 📦 Estructura del Proyecto    "stock": 15

  }'

```

PruebaAPI/# Actualizar un producto

├── Domain/                    # Capa de Dominiocurl -X PUT http://localhost:8080/api/products/1 \

│   ├── Entities/             # Entidades del negocio  -H "Content-Type: application/json" \

│   └── Interfaces/           # Contratos del dominio  -d '{

├── Application/              # Capa de Aplicación    "id": 1,

│   ├── DTOs/                # Objetos de transferencia de datos    "name": "Laptop Pro",

│   ├── Interfaces/          # Contratos de servicios    "description": "Updated description",

│   └── Services/            # Lógica de aplicación    "price": 1499.99,

├── Infrastructure/           # Capa de Infraestructura    "stock": 8,

│   ├── Persistence/         # DbContext y configuraciones EF    "createdAt": "2025-10-25T00:00:00Z"

│   └── Repositories/        # Implementaciones de repositorios  }'

├── Presentation/            # Capa de Presentación

│   └── Controllers/         # Controladores API REST# Eliminar un producto

└── Migrations/              # Migraciones de base de datoscurl -X DELETE http://localhost:8080/api/products/1

``````



## 🧪 Datos de Prueba### Con Swagger UI



La aplicación incluye datos seed de 5 marcas famosas:1. Abre http://localhost:8080/swagger

2. Explora los endpoints disponibles

1. **Toyota** (Japón, 1937)3. Haz clic en "Try it out" para probar las peticiones

2. **Ford** (Estados Unidos, 1903)

3. **BMW** (Alemania, 1916)## 🛠️ Desarrollo Local (sin Docker)

4. **Ferrari** (Italia, 1939)

5. **Tesla** (Estados Unidos, 2003)### 1. Instalar dependencias



## 🔧 Tecnologías Utilizadas```bash

cd PruebaAPI

- **Framework**: .NET 8dotnet restore

- **ORM**: Entity Framework Core 9.0```

- **Base de Datos**: PostgreSQL 16

- **Contenedores**: Docker & Docker Compose### 2. Configurar la base de datos

- **Documentación**: Swagger/OpenAPI

- **Testing**: xUnit, FluentAssertions, MoqAsegúrate de tener PostgreSQL corriendo localmente y actualiza `appsettings.json`:



## 📚 Documentación Adicional```json

{

Para entender la arquitectura del proyecto y los patrones implementados, consulta:  "ConnectionStrings": {

    "DefaultConnection": "Host=localhost;Port=5432;Database=pruebaapi;Username=postgres;Password=tu_password"

- [ARQUITECTURA.md](./ARQUITECTURA.md) - Explicación detallada de Clean Architecture y patrones  }

}

## 🐛 Troubleshooting```



### La API no responde### 3. Ejecutar migraciones



```bash```bash

# Verificar que los contenedores estén corriendodotnet ef migrations add InitialCreate

docker psdotnet ef database update

```

# Ver logs de la API

docker logs pruebaapi-api### 4. Ejecutar la aplicación



# Ver logs de PostgreSQL```bash

docker logs pruebaapi-postgresdotnet run

``````



### Error de conexión a la base de datosLa API estará disponible en http://localhost:5000



```bash## 🐳 Comandos Docker Útiles

# Reiniciar los contenedores

docker-compose down```bash

docker-compose up -d# Ver logs de los contenedores

docker-compose logs -f

# Esperar a que PostgreSQL esté listo (toma ~10 segundos)

docker logs pruebaapi-postgres | grep "ready to accept connections"# Ver logs solo de la API

```docker-compose logs -f api



### Limpiar todo y empezar de nuevo# Ver logs solo de PostgreSQL

docker-compose logs -f postgres

```bash

# Detener y eliminar contenedores, volúmenes e imágenes# Detener los contenedores

docker-compose down -vdocker-compose down

docker-compose up -d --build

```# Detener y eliminar volúmenes (datos de BD)

docker-compose down -v

## 📄 Licencia

# Reiniciar un servicio específico

Este proyecto es código abierto y está disponible bajo la licencia MIT.docker-compose restart api



## 👥 Contribuciones# Ejecutar comandos en el contenedor

docker-compose exec api bash

Las contribuciones son bienvenidas. Por favor:docker-compose exec postgres psql -U postgres -d pruebaapi

```

1. Fork el proyecto

2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)## 📦 Variables de Entorno

3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)

4. Push a la rama (`git push origin feature/AmazingFeature`)### API (en docker-compose.yml)

5. Abre un Pull Request

- `ASPNETCORE_ENVIRONMENT`: Entorno de ejecución (Development/Production)

## 📞 Soporte- `ConnectionStrings__DefaultConnection`: String de conexión a PostgreSQL



Si encuentras algún problema o tienes preguntas, por favor abre un issue en el repositorio.### PostgreSQL



---- `POSTGRES_DB`: Nombre de la base de datos

- `POSTGRES_USER`: Usuario de PostgreSQL

**Desarrollado con ❤️ usando Clean Architecture y .NET 8**- `POSTGRES_PASSWORD`: Contraseña de PostgreSQL


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
