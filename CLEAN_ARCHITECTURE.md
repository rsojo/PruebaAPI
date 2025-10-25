# Clean Architecture - Documentación Completa

## 🏛️ Introducción a Clean Architecture

Clean Architecture (Arquitectura Limpia) es un patrón de diseño de software propuesto por Robert C. Martin (Uncle Bob) que separa el código en capas concéntricas, donde las capas internas no conocen las externas.

### Principios Fundamentales

1. **Independencia de Frameworks**: El negocio no depende de frameworks
2. **Testabilidad**: Lógica de negocio fácil de probar sin UI, BD o servicios externos
3. **Independencia de UI**: La UI puede cambiar sin afectar el negocio
4. **Independencia de BD**: Puedes cambiar PostgreSQL por MongoDB sin afectar el negocio
5. **Independencia de Agentes Externos**: El negocio no conoce el mundo exterior

---

## 📊 Estructura de Capas

```
┌─────────────────────────────────────────────────┐
│              Presentation Layer                  │
│         (Controllers, API Endpoints)             │
│  - ProductsController                            │
│  - Maneja HTTP requests/responses                │
└────────────────┬────────────────────────────────┘
                 │ depende de ↓
┌─────────────────────────────────────────────────┐
│             Application Layer                    │
│      (Use Cases, Services, DTOs)                 │
│  - IProductService / ProductService              │
│  - CreateProductDto, UpdateProductDto            │
│  - Lógica de aplicación y casos de uso          │
└────────────────┬────────────────────────────────┘
                 │ depende de ↓
┌─────────────────────────────────────────────────┐
│               Domain Layer                       │
│       (Entities, Interfaces Core)                │
│  - Product (Entity)                              │
│  - IRepository, IProductRepository               │
│  - IUnitOfWork                                   │
│  - NO depende de nada ✅                         │
└────────────────┬────────────────────────────────┘
                 │ ↑ implementado por
┌─────────────────────────────────────────────────┐
│            Infrastructure Layer                  │
│    (Data Access, External Services)              │
│  - AppDbContext (EF Core)                        │
│  - Repository, ProductRepository                 │
│  - UnitOfWork                                    │
│  - Configuraciones de BD                         │
└─────────────────────────────────────────────────┘
```

---

## 🎯 Capas Detalladas

### 1. **Domain Layer** (Núcleo del Negocio)

**Ubicación**: `Domain/`

**Responsabilidades**:
- Define las entidades del dominio
- Define interfaces (contratos) sin implementación
- Contiene la lógica de negocio pura
- **NO tiene dependencias externas**

**Estructura**:
```
Domain/
├── Entities/
│   └── Product.cs                 # Entidad de dominio
└── Interfaces/
    ├── IRepository.cs            # Contrato genérico
    ├── IProductRepository.cs     # Contrato específico
    └── IUnitOfWork.cs            # Patrón Unit of Work
```

**Código Ejemplo**:
```csharp
namespace PruebaAPI.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
```

---

### 2. **Application Layer** (Casos de Uso)

**Ubicación**: `Application/`

**Responsabilidades**:
- Implementa casos de uso de la aplicación
- Define DTOs para transferencia de datos
- Orquesta el flujo de datos entre capas
- **Depende solo de Domain**

**Estructura**:
```
Application/
├── DTOs/
│   ├── ProductDto.cs             # DTO de lectura
│   ├── CreateProductDto.cs       # DTO de creación
│   └── UpdateProductDto.cs       # DTO de actualización
├── Interfaces/
│   └── IProductService.cs        # Interfaz del servicio
└── Services/
    └── ProductService.cs         # Implementación de casos de uso
```

**Código Ejemplo**:
```csharp
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product 
        { 
            Name = dto.Name, 
            Price = dto.Price 
        };
        
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        
        return MapToDto(product);
    }
}
```

**Ventajas de usar DTOs**:
- ✅ Oculta detalles internos de las entidades
- ✅ Previene over-posting attacks
- ✅ Permite evolucionar la API independientemente del dominio
- ✅ Facilita versionamiento de API

---

### 3. **Infrastructure Layer** (Implementación)

**Ubicación**: `Infrastructure/`

**Responsabilidades**:
- Implementa interfaces definidas en Domain
- Acceso a base de datos (EF Core)
- Servicios externos (APIs, email, etc.)
- **Depende de Domain**

**Estructura**:
```
Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs           # DbContext de EF Core
│   └── Configurations/
│       └── ProductConfiguration.cs # Configuración de entidad
└── Repositories/
    ├── Repository.cs             # Implementación genérica
    ├── ProductRepository.cs      # Implementación específica
    └── UnitOfWork.cs             # Implementación Unit of Work
```

**Código Ejemplo - Unit of Work**:
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IProductRepository? _productRepository;

    public IProductRepository Products
    {
        get => _productRepository ??= new ProductRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync() { ... }
    public async Task CommitTransactionAsync() { ... }
}
```

**Patrón Unit of Work**:
- ✅ Centraliza operaciones de guardado
- ✅ Soporta transacciones
- ✅ Un solo punto para SaveChanges()
- ✅ Gestión consistente de cambios

---

### 4. **Presentation Layer** (Interfaz)

**Ubicación**: `Presentation/`

**Responsabilidades**:
- Expone la API HTTP
- Maneja requests y responses
- Validación de entrada
- **Depende de Application**

**Estructura**:
```
Presentation/
└── Controllers/
    └── ProductsController.cs     # Controlador REST
```

**Código Ejemplo**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductDto dto)
    {
        var product = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetProduct), 
            new { id = product.Id }, product);
    }
}
```

---

## 🔄 Flujo de Datos Completo

### Ejemplo: Crear un Producto

```
1. HTTP POST /api/products
   └─> ProductsController.CreateProduct(CreateProductDto)
       
2. Controller llama al servicio de aplicación
   └─> IProductService.CreateProductAsync(dto)
       
3. Servicio mapea DTO a Entidad de Domain
   └─> var product = new Product { ... }
       
4. Servicio usa Unit of Work para persistir
   └─> await _unitOfWork.Products.AddAsync(product)
   └─> await _unitOfWork.SaveChangesAsync()
       
5. Repository interactúa con Infrastructure
   └─> AppDbContext.Products.Add(product)
   └─> AppDbContext.SaveChanges() → PostgreSQL
       
6. Servicio mapea Entidad a DTO de respuesta
   └─> return MapToDto(product)
       
7. Controller retorna HTTP 201 Created
   └─> return CreatedAtAction(..., ProductDto)
```

---

## 🎯 Ventajas de esta Arquitectura

### 1. **Separación de Responsabilidades**
Cada capa tiene un propósito claro y único.

### 2. **Testabilidad Máxima**
```csharp
// Test unitario del servicio
var mockUnitOfWork = new Mock<IUnitOfWork>();
var service = new ProductService(mockUnitOfWork.Object);

// Test sin base de datos real ✅
```

### 3. **Flexibilidad**
Puedes cambiar:
- Base de datos (PostgreSQL → MongoDB)
- Framework web (ASP.NET → FastAPI)
- UI (REST → GraphQL → gRPC)

Sin afectar la lógica de negocio.

### 4. **Mantenibilidad**
Código organizado y fácil de navegar.

### 5. **Escalabilidad**
Fácil agregar nuevas features siguiendo el patrón establecido.

---

## 📝 Patrones Implementados

### 1. **Repository Pattern**
Abstrae el acceso a datos detrás de interfaces.

### 2. **Unit of Work Pattern**
Mantiene consistencia en operaciones transaccionales.

### 3. **Dependency Injection**
Todas las dependencias se inyectan, facilitando testing.

### 4. **DTO Pattern**
Separa modelos de dominio de contratos de API.

### 5. **Service Layer Pattern**
Encapsula lógica de aplicación en servicios reutilizables.

---

## 🔧 Configuración de Dependency Injection

**Program.cs**:
```csharp
// Infrastructure Layer
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application Layer
builder.Services.AddScoped<IProductService, ProductService>();

// Presentation Layer
builder.Services.AddControllers();
```

**Orden de registro**:
1. Infrastructure (DbContext, Repositories)
2. Application (Services)
3. Presentation (Controllers)

---

## 🚀 Cómo Extender la Aplicación

### Agregar una nueva entidad (ejemplo: Category)

#### 1. Domain Layer
```csharp
// Domain/Entities/Category.cs
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Domain/Interfaces/ICategoryRepository.cs
public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetActiveCategories();
}
```

#### 2. Application Layer
```csharp
// Application/DTOs/CategoryDto.cs
public class CategoryDto { ... }

// Application/Interfaces/ICategoryService.cs
public interface ICategoryService { ... }

// Application/Services/CategoryService.cs
public class CategoryService : ICategoryService { ... }
```

#### 3. Infrastructure Layer
```csharp
// Infrastructure/Persistence/Configurations/CategoryConfiguration.cs
public class CategoryConfiguration : IEntityTypeConfiguration<Category> { ... }

// Infrastructure/Repositories/CategoryRepository.cs
public class CategoryRepository : Repository<Category>, ICategoryRepository { ... }

// Actualizar UnitOfWork.cs
public ICategoryRepository Categories { get; }
```

#### 4. Presentation Layer
```csharp
// Presentation/Controllers/CategoriesController.cs
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase { ... }
```

#### 5. Registrar en DI (Program.cs)
```csharp
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
```

---

## 📊 Comparación: Antes vs Después

| Aspecto | Arquitectura Anterior | Clean Architecture |
|---------|----------------------|-------------------|
| Capas | 3 (Controller, Repository, Data) | 4 (Presentation, Application, Domain, Infrastructure) |
| Dependencias | Controllers → DbContext directamente | Controllers → Services → Repositories |
| DTOs | No (expone entidades) | Sí (CreateDto, UpdateDto, ReadDto) |
| Unit of Work | No | Sí ✅ |
| Testabilidad | Media | Alta ✅ |
| Separación | Moderada | Excelente ✅ |
| Complejidad | Baja | Media (pero vale la pena) |

---

## 🎓 Principios SOLID Aplicados

### **S** - Single Responsibility
Cada clase tiene una única razón para cambiar.

### **O** - Open/Closed
Abierto para extensión, cerrado para modificación.

### **L** - Liskov Substitution
Las implementaciones pueden sustituir interfaces sin problemas.

### **I** - Interface Segregation
Interfaces específicas por necesidad (IProductService, IProductRepository).

### **D** - Dependency Inversion
Dependemos de abstracciones (interfaces), no de implementaciones concretas.

---

## 📚 Referencias

- **Clean Architecture** - Robert C. Martin (Uncle Bob)
- **Domain-Driven Design** - Eric Evans
- **Patterns of Enterprise Application Architecture** - Martin Fowler
- [Microsoft Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)

---

## ✅ Checklist de Implementación

- [x] Domain Layer con entidades e interfaces
- [x] Application Layer con DTOs y servicios
- [x] Infrastructure Layer con EF Core y repositorios
- [x] Presentation Layer con controladores
- [x] Unit of Work implementado
- [x] Dependency Injection configurada
- [x] Migraciones de base de datos
- [x] Documentación completa

---

**Implementado por**: Clean Architecture Pattern
**Fecha**: 25 de Octubre, 2025
**Estado**: ✅ Producción Ready
