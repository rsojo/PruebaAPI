# Arquitectura del Proyecto - Patrón Repository

## 📐 Descripción General

Este proyecto implementa el **Patrón Repository** para abstraer la lógica de acceso a datos y proporcionar una interfaz más limpia y mantenible para trabajar con Entity Framework Core.

## 🏛️ Capas de la Arquitectura

### 1. **Capa de Presentación (Controllers)**
- `ProductsController.cs`: Maneja las peticiones HTTP y respuestas
- Depende de `IProductRepository` (no del DbContext directamente)
- Responsable de validación de entrada y formateo de respuestas

### 2. **Capa de Repositorio (Repositories)**

#### Repositorio Genérico
- **`IRepository<T>`**: Interfaz genérica con operaciones CRUD básicas
- **`Repository<T>`**: Implementación base con lógica común reutilizable

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(int id);
    Task<int> SaveChangesAsync();
}
```

#### Repositorio Específico
- **`IProductRepository`**: Interfaz con métodos específicos del dominio
- **`ProductRepository`**: Implementación con lógica de negocio especializada

```csharp
public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Product>> GetProductsInStockAsync();
    Task<IEnumerable<Product>> SearchByNameAsync(string name);
}
```

### 3. **Capa de Acceso a Datos (Data)**

#### DbContext
- **`AppDbContext`**: Contexto de Entity Framework Core
- Aplica configuraciones desde archivos separados automáticamente
- No contiene lógica de negocio

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
```

#### Configuraciones de Entidades
- **`ProductConfiguration.cs`**: Configuración de la entidad Product
- Implementa `IEntityTypeConfiguration<Product>`
- Define:
  - Claves primarias
  - Propiedades y restricciones
  - Índices
  - Datos de seed

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(e => e.Name).HasDatabaseName("IX_Products_Name");
        // ... más configuraciones
    }
}
```

### 4. **Capa de Modelos (Models)**
- **`Product.cs`**: Entidad de dominio (POCO)
- Sin lógica de negocio, solo propiedades

## 🎯 Ventajas del Patrón Repository

### 1. **Separación de Responsabilidades**
- Controladores no conocen detalles de EF Core
- Repositorios encapsulan la lógica de acceso a datos
- Configuraciones separadas mantienen el código organizado

### 2. **Testabilidad**
```csharp
// Fácil de mockear para pruebas unitarias
var mockRepo = new Mock<IProductRepository>();
mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
var controller = new ProductsController(mockRepo.Object, logger);
```

### 3. **Mantenibilidad**
- Cambios en la lógica de datos no afectan controladores
- Fácil agregar nuevos métodos de consulta
- Configuraciones centralizadas y reutilizables

### 4. **Reutilización**
- El repositorio genérico se puede usar con cualquier entidad
- Métodos comunes implementados una sola vez
- Configuraciones aplicables a nuevas entidades

## 🔄 Flujo de Datos

```
HTTP Request
    ↓
ProductsController
    ↓
IProductRepository (interfaz)
    ↓
ProductRepository (implementación)
    ↓
AppDbContext
    ↓
PostgreSQL Database
```

## 🛠️ Inyección de Dependencias

En `Program.cs`:

```csharp
// Registro del repositorio genérico
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Registro del repositorio específico
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

## 📦 Estructura de Archivos

```
PruebaAPI/
├── Controllers/
│   └── ProductsController.cs           # Usa IProductRepository
├── Repositories/
│   ├── IRepository.cs                  # Interfaz genérica
│   ├── Repository.cs                   # Implementación genérica
│   ├── IProductRepository.cs           # Interfaz específica
│   └── ProductRepository.cs            # Implementación específica
├── Data/
│   ├── AppDbContext.cs                 # Contexto limpio
│   └── Configurations/
│       └── ProductConfiguration.cs     # Configuración separada
└── Models/
    └── Product.cs                      # Entidad de dominio
```

## 🚀 Extensibilidad

### Agregar una nueva entidad

1. **Crear el modelo**:
```csharp
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

2. **Crear la configuración**:
```csharp
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Configuración...
    }
}
```

3. **Crear el repositorio** (opcional):
```csharp
public interface ICategoryRepository : IRepository<Category>
{
    // Métodos específicos...
}

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    // Implementación...
}
```

4. **Registrar en DI**:
```csharp
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
```

5. **Crear el controlador**:
```csharp
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _repository;
    // ...
}
```

## 🎓 Mejores Prácticas Implementadas

- ✅ **DRY (Don't Repeat Yourself)**: Código reutilizable en Repository<T>
- ✅ **SOLID Principles**:
  - **S**ingle Responsibility: Cada clase tiene una responsabilidad clara
  - **O**pen/Closed: Abierto a extensión, cerrado a modificación
  - **L**iskov Substitution: Las implementaciones pueden sustituir interfaces
  - **I**nterface Segregation: Interfaces específicas por dominio
  - **D**ependency Inversion: Depende de abstracciones, no implementaciones
- ✅ **Async/Await**: Operaciones asíncronas para mejor rendimiento
- ✅ **Dependency Injection**: Inyección de dependencias nativa de .NET
- ✅ **Configuration as Code**: Configuraciones en archivos dedicados

## 📚 Referencias

- [Repository Pattern - Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Entity Type Configuration - EF Core](https://docs.microsoft.com/en-us/ef/core/modeling/)
- [Dependency Injection in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
