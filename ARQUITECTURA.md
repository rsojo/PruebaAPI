# 🏛️ Arquitectura del Proyecto

Este documento explica en detalle la arquitectura, patrones de diseño y decisiones técnicas implementadas en la API de Marcas de Autos.

## 📐 Clean Architecture

### ¿Qué es Clean Architecture?

Clean Architecture es un patrón arquitectónico propuesto por Robert C. Martin (Uncle Bob) que organiza el código en capas concéntricas, donde las dependencias apuntan hacia el centro (las reglas de negocio).

### Principios Fundamentales

1. **Independencia de Frameworks**: La arquitectura no depende de bibliotecas externas
2. **Testabilidad**: La lógica de negocio puede probarse sin UI, DB o frameworks
3. **Independencia de UI**: La UI puede cambiar sin afectar el sistema
4. **Independencia de Base de Datos**: Se puede cambiar PostgreSQL por SQL Server sin afectar las reglas de negocio
5. **Independencia de Agentes Externos**: Las reglas de negocio no saben nada del mundo exterior

### Las 4 Capas

```
┌─────────────────────────────────────────────┐
│           Presentation Layer                │  ← API Controllers
│         (Controladores REST)                │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│          Application Layer                  │  ← Use Cases, DTOs
│    (Servicios, Lógica de Aplicación)       │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│            Domain Layer                     │  ← Entidades, Interfaces
│        (Núcleo del Negocio)                 │  ← NO DEPENDE DE NADIE
└──────────────────▲──────────────────────────┘
                   │
┌──────────────────┴──────────────────────────┐
│        Infrastructure Layer                 │  ← EF Core, Repositorios
│    (Implementaciones Concretas)             │
└─────────────────────────────────────────────┘
```

## 🎯 Capas del Proyecto

### 1️⃣ Domain Layer (Capa de Dominio)

**Ubicación**: `PruebaAPI/Domain/`

**Responsabilidad**: Contiene las **reglas de negocio** puras y las **entidades del dominio**.

**Características**:
- ✅ No tiene dependencias externas
- ✅ Contiene las entidades del negocio
- ✅ Define interfaces (contratos) para repositorios
- ✅ Es el corazón del sistema

**Archivos**:

```
Domain/
├── Entities/
│   └── MarcaAuto.cs          # Entidad principal del dominio
└── Interfaces/
    ├── IRepository.cs        # Contrato genérico de repositorio
    ├── IMarcaAutoRepository.cs  # Contrato específico
    └── IUnitOfWork.cs        # Contrato de Unit of Work
```

**Ejemplo - MarcaAuto.cs**:
```csharp
public class MarcaAuto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? PaisOrigen { get; set; }
    public int AñoFundacion { get; set; }
    public string? SitioWeb { get; set; }
    public bool EsActiva { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
```

### 2️⃣ Application Layer (Capa de Aplicación)

**Ubicación**: `PruebaAPI/Application/`

**Responsabilidad**: Contiene la **lógica de aplicación** y los **casos de uso**.

**Características**:
- ✅ Orquesta el flujo de datos
- ✅ Usa DTOs para comunicación externa
- ✅ Implementa los casos de uso del negocio
- ✅ No sabe de bases de datos ni UI

**Archivos**:

```
Application/
├── DTOs/
│   ├── MarcaAutoDto.cs       # DTO de lectura
│   ├── CreateMarcaAutoDto.cs # DTO de creación
│   └── UpdateMarcaAutoDto.cs # DTO de actualización
├── Interfaces/
│   └── IMarcaAutoService.cs  # Contrato de servicio
└── Services/
    └── MarcaAutoService.cs   # Implementación de lógica
```

**Ejemplo - MarcaAutoService.cs**:
```csharp
public class MarcaAutoService : IMarcaAutoService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<MarcaAutoDto> CreateMarcaAsync(CreateMarcaAutoDto dto)
    {
        // Lógica de negocio
        var marca = MapToEntity(dto);
        await _unitOfWork.MarcasAutos.AddAsync(marca);
        await _unitOfWork.SaveChangesAsync();
        return MapToDto(marca);
    }
}
```

**¿Por qué usar DTOs?**
- ✅ Separan el modelo de dominio del contrato de API
- ✅ Previenen over-posting (envío de campos no deseados)
- ✅ Permiten versionado de API sin cambiar el dominio
- ✅ Ocultan detalles internos del modelo

### 3️⃣ Infrastructure Layer (Capa de Infraestructura)

**Ubicación**: `PruebaAPI/Infrastructure/`

**Responsabilidad**: Implementa los detalles técnicos (**EF Core, Base de Datos**).

**Características**:
- ✅ Implementa las interfaces del dominio
- ✅ Contiene configuraciones de EF Core
- ✅ Gestiona el acceso a datos
- ✅ Puede ser reemplazada sin afectar el dominio

**Archivos**:

```
Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   └── Configurations/
│       └── MarcaAutoConfiguration.cs
└── Repositories/
    ├── Repository.cs          # Repositorio genérico
    ├── MarcaAutoRepository.cs # Repositorio específico
    └── UnitOfWork.cs          # Implementación de UoW
```

### 4️⃣ Presentation Layer (Capa de Presentación)

**Ubicación**: `PruebaAPI/Presentation/`

**Responsabilidad**: Expone la API REST y maneja las peticiones HTTP.

**Características**:
- ✅ Controladores REST
- ✅ Validación de entrada
- ✅ Códigos de estado HTTP
- ✅ Documentación Swagger

**Archivos**:

```
Presentation/
└── Controllers/
    └── MarcasAutosController.cs
```

## 🎨 Patrones de Diseño Implementados

### 1. Repository Pattern

**¿Qué es?**

El patrón Repository abstrae el acceso a datos, proporcionando una interfaz para trabajar con entidades sin exponer detalles de la base de datos.

**Ventajas**:
- ✅ Desacopla la lógica de negocio de la lógica de acceso a datos
- ✅ Facilita el testing (se puede mockear)
- ✅ Centraliza las consultas a la base de datos
- ✅ Permite cambiar el ORM sin afectar el negocio

**Implementación**:

```csharp
// Interfaz genérica (Domain)
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

// Interfaz específica (Domain)
public interface IMarcaAutoRepository : IRepository<MarcaAuto>
{
    Task<IEnumerable<MarcaAuto>> SearchByNameAsync(string nombre);
    Task<IEnumerable<MarcaAuto>> GetMarcasActivasAsync();
}

// Implementación (Infrastructure)
public class MarcaAutoRepository : Repository<MarcaAuto>, IMarcaAutoRepository
{
    public async Task<IEnumerable<MarcaAuto>> SearchByNameAsync(string nombre)
    {
        return await _context.Set<MarcaAuto>()
            .Where(m => m.Nombre.ToLower().Contains(nombre.ToLower()))
            .ToListAsync();
    }
}
```

### 2. Unit of Work Pattern

**¿Qué es?**

Unit of Work mantiene una lista de objetos afectados por una transacción de negocio y coordina la escritura de cambios.

**¿Por qué usar Unit of Work?**

#### ❌ Sin Unit of Work:

```csharp
// Problema: Múltiples llamadas a SaveChanges
public async Task CreateOrder(Order order)
{
    await _orderRepository.AddAsync(order);
    await _context.SaveChangesAsync();  // SaveChanges #1
    
    await _inventoryRepository.UpdateStock(order.ProductId);
    await _context.SaveChangesAsync();  // SaveChanges #2
    
    await _notificationRepository.SendNotification();
    await _context.SaveChangesAsync();  // SaveChanges #3
}

// Problemas:
// 1. Si falla el paso 2, el paso 1 ya se guardó (inconsistencia)
// 2. Multiple round-trips a la base de datos
// 3. Difícil de hacer rollback
```

#### ✅ Con Unit of Work:

```csharp
public async Task CreateOrder(Order order)
{
    await _unitOfWork.Orders.AddAsync(order);
    await _unitOfWork.Inventory.UpdateStock(order.ProductId);
    await _unitOfWork.Notifications.SendNotification();
    
    // Una sola llamada a SaveChanges
    await _unitOfWork.SaveChangesAsync();  // Todo o nada (ACID)
}
```

**Ventajas del Unit of Work**:

1. **Transaccionalidad**: Todo o nada (principio ACID)
2. **Eficiencia**: Un solo viaje a la base de datos
3. **Consistencia**: Los datos se guardan juntos
4. **Rollback fácil**: Si algo falla, nada se guarda
5. **Control de transacciones**: `BeginTransaction`, `Commit`, `Rollback`

**Implementación**:

```csharp
public interface IUnitOfWork : IDisposable
{
    IMarcaAutoRepository MarcasAutos { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IMarcaAutoRepository? _marcaAutoRepository;
    
    public IMarcaAutoRepository MarcasAutos
    {
        get => _marcaAutoRepository ??= new MarcaAutoRepository(_context);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
```

**Ejemplo de uso en Service**:

```csharp
public class MarcaAutoService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<bool> TransferirMarca(int marcaId, string nuevoPais)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var marca = await _unitOfWork.MarcasAutos.GetByIdAsync(marcaId);
            marca.PaisOrigen = nuevoPais;
            
            await _unitOfWork.MarcasAutos.UpdateAsync(marca);
            await _unitOfWork.SaveChangesAsync();
            
            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            return false;
        }
    }
}
```

### 3. Dependency Injection (DI)

**¿Qué es?**

Inyección de Dependencias permite que las clases reciban sus dependencias desde el exterior en lugar de crearlas internamente.

**Configuración en Program.cs**:

```csharp
// Infrastructure
builder.Services.AddScoped<IMarcaAutoRepository, MarcaAutoRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application
builder.Services.AddScoped<IMarcaAutoService, MarcaAutoService>();
```

**Ventajas**:
- ✅ Facilita el testing (inyección de mocks)
- ✅ Reduce el acoplamiento
- ✅ Aumenta la mantenibilidad
- ✅ Facilita el cambio de implementaciones

## 🗄️ Entity Framework Core: Configuraciones Fluidas

### ¿Por qué usar configuraciones separadas?

#### ❌ Sin configuraciones (solo atributos):

```csharp
public class MarcaAuto
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; }
    
    // Problema: Mezcla dominio con infraestructura
    // La entidad de dominio no debería saber de EF Core
}
```

#### ✅ Con configuraciones fluidas:

```csharp
// Entidad limpia (Domain)
public class MarcaAuto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    // Sin atributos de EF Core
}

// Configuración separada (Infrastructure)
public class MarcaAutoConfiguration : IEntityTypeConfiguration<MarcaAuto>
{
    public void Configure(EntityTypeBuilder<MarcaAuto> builder)
    {
        builder.ToTable("MarcasAutos");
        
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Nombre)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.HasIndex(m => m.Nombre);
        
        // Seed data
        builder.HasData(
            new MarcaAuto { Id = 1, Nombre = "Toyota", /* ... */ }
        );
    }
}
```

### Ventajas de Configuraciones Fluidas:

1. **Separación de Responsabilidades**
   - El dominio no conoce detalles de persistencia
   - Cambios en BD no afectan el dominio

2. **Flexibilidad**
   - Configuración más potente que atributos
   - Se puede configurar relaciones complejas

3. **Mantenibilidad**
   - Cada entidad tiene su archivo de configuración
   - Fácil de encontrar y modificar

4. **Testabilidad**
   - El dominio se puede testear sin EF Core
   - Tests más rápidos y simples

5. **Seed Data Centralizado**
   - Datos de prueba en un solo lugar
   - Versionados con migraciones

### Aplicación de Configuraciones

```csharp
public class AppDbContext : DbContext
{
    public DbSet<MarcaAuto> MarcasAutos { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas las configuraciones automáticamente
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```

## 🔄 Flujo de una Petición

```
1. HTTP Request
   ↓
2. MarcasAutosController (Presentation)
   ↓
3. MarcaAutoService (Application)
   - Valida lógica de negocio
   - Convierte DTOs ↔ Entities
   ↓
4. UnitOfWork → MarcaAutoRepository (Infrastructure)
   - Ejecuta consultas en BD
   ↓
5. Entity Framework Core
   ↓
6. PostgreSQL Database
   ↓
7. Respuesta en sentido inverso
```

## 🧪 Testing

La arquitectura facilita el testing en cada capa:

### Unit Tests de Domain
```csharp
// No necesita base de datos
[Fact]
public void MarcaAuto_Should_AllowValidData()
{
    var marca = new MarcaAuto { Nombre = "Toyota", AñoFundacion = 1937 };
    marca.Nombre.Should().Be("Toyota");
}
```

### Unit Tests de Application (con mocks)
```csharp
var mockUnitOfWork = new Mock<IUnitOfWork>();
var service = new MarcaAutoService(mockUnitOfWork.Object);
```

### Integration Tests
```csharp
// Usa InMemory o TestContainers
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase("TestDb")
    .Options;
```

## 📊 Ventajas de esta Arquitectura

| Aspecto | Beneficio |
|---------|-----------|
| **Mantenibilidad** | Código organizado y fácil de entender |
| **Testabilidad** | Cada capa se puede testear independientemente |
| **Escalabilidad** | Fácil agregar nuevas features |
| **Flexibilidad** | Se puede cambiar UI, BD o frameworks |
| **Reutilización** | La lógica de negocio se puede usar en otros proyectos |
| **Colaboración** | Equipos pueden trabajar en paralelo por capas |

## 🚀 Extensibilidad

### Agregar una nueva entidad

1. Crear entidad en `Domain/Entities/`
2. Crear interface en `Domain/Interfaces/`
3. Crear DTOs en `Application/DTOs/`
4. Crear service en `Application/Services/`
5. Crear configuration en `Infrastructure/Persistence/Configurations/`
6. Crear repository en `Infrastructure/Repositories/`
7. Actualizar `IUnitOfWork` y `UnitOfWork`
8. Crear controller en `Presentation/Controllers/`
9. Crear migración

### Cambiar de base de datos

Solo necesitas cambiar:
- Connection string en `appsettings.json`
- Provider en `Program.cs` (`.UseNpgsql()` → `.UseSqlServer()`)

El resto del código **NO cambia** ✅

## 📚 Referencias

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Repository Pattern - Martin Fowler](https://martinfowler.com/eaaCatalog/repository.html)
- [Unit of Work - Martin Fowler](https://martinfowler.com/eaaCatalog/unitOfWork.html)
- [Entity Framework Core - Microsoft Docs](https://docs.microsoft.com/ef/core/)

---

**Esta arquitectura está diseñada para proyectos que crecen y necesitan mantenerse por años** 🏗️
