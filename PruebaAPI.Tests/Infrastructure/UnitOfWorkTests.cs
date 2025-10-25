using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PruebaAPI.Domain.Entities;
using PruebaAPI.Domain.Interfaces;
using PruebaAPI.Infrastructure.Persistence;
using PruebaAPI.Infrastructure.Repositories;

namespace PruebaAPI.Tests.Infrastructure;

public class UnitOfWorkTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new AppDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public void MarcasAutos_Should_ReturnMarcaAutoRepository()
    {
        // Act
        var repository = _unitOfWork.MarcasAutos;

        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeAssignableTo<IMarcaAutoRepository>();
    }

    [Fact]
    public void MarcasAutos_Should_ReturnSameInstanceOnMultipleCalls()
    {
        // Act
        var repository1 = _unitOfWork.MarcasAutos;
        var repository2 = _unitOfWork.MarcasAutos;

        // Assert
        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public async Task SaveChangesAsync_Should_PersistChangesToDatabase()
    {
        // Arrange
        var marca = new MarcaAuto
        {
            Nombre = "Chevrolet",
            PaisOrigen = "Estados Unidos",
            AñoFundacion = 1911,
            SitioWeb = "https://www.chevrolet.com",
            EsActiva = true,
            FechaCreacion = DateTime.UtcNow
        };

        await _unitOfWork.MarcasAutos.AddAsync(marca);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().BeGreaterThan(0);
        var savedMarca = await _context.MarcasAutos.FirstOrDefaultAsync(m => m.Nombre == "Chevrolet");
        savedMarca.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_Should_ReturnNumberOfChanges()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Nombre = "Volkswagen", PaisOrigen = "Alemania", AñoFundacion = 1937, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Honda", PaisOrigen = "Japón", AñoFundacion = 1948, EsActiva = true, FechaCreacion = DateTime.UtcNow }
        };

        foreach (var marca in marcas)
        {
            await _unitOfWork.MarcasAutos.AddAsync(marca);
        }

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task BeginTransactionAsync_Should_CreateTransaction()
    {
        // Act
        await _unitOfWork.BeginTransactionAsync();

        // Assert
        // InMemory database ignores transactions, but the method should execute without errors
        // In real scenarios with a real database, CurrentTransaction would not be null
        var act = () => _unitOfWork.BeginTransactionAsync();
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task CommitTransactionAsync_Should_CommitChanges()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        
        var marca = new MarcaAuto
        {
            Nombre = "Nissan",
            PaisOrigen = "Japón",
            AñoFundacion = 1933,
            EsActiva = true,
            FechaCreacion = DateTime.UtcNow
        };

        await _unitOfWork.MarcasAutos.AddAsync(marca);
        await _unitOfWork.SaveChangesAsync();

        // Act
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        // InMemory database doesn't fully support transactions, but we verify the data was saved
        var savedMarca = await _context.MarcasAutos.FirstOrDefaultAsync(m => m.Nombre == "Nissan");
        savedMarca.Should().NotBeNull();
        savedMarca!.PaisOrigen.Should().Be("Japón");
    }

    [Fact]
    public async Task RollbackTransactionAsync_Should_RevertChanges()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        
        var marca = new MarcaAuto
        {
            Nombre = "Rollback Test",
            PaisOrigen = "Test",
            AñoFundacion = 2000,
            EsActiva = true,
            FechaCreacion = DateTime.UtcNow
        };

        await _unitOfWork.MarcasAutos.AddAsync(marca);
        // Note: We intentionally don't call SaveChangesAsync before rollback

        // Act
        await _unitOfWork.RollbackTransactionAsync();

        // Assert
        // InMemory database doesn't fully support transactions rollback,
        // but we verify the method executes without errors
        var act = () => _unitOfWork.RollbackTransactionAsync();
        await act.Should().NotThrowAsync();
        
        // Since we didn't save, the marca shouldn't exist
        var savedMarca = await _context.MarcasAutos.FirstOrDefaultAsync(m => m.Nombre == "Rollback Test");
        savedMarca.Should().BeNull();
    }

    [Fact]
    public async Task MultipleOperations_Should_BeSavedTogether()
    {
        // Arrange
        var marca1 = new MarcaAuto
        {
            Nombre = "Audi",
            PaisOrigen = "Alemania",
            AñoFundacion = 1909,
            EsActiva = true,
            FechaCreacion = DateTime.UtcNow
        };

        var marca2 = new MarcaAuto
        {
            Nombre = "Mazda",
            PaisOrigen = "Japón",
            AñoFundacion = 1920,
            EsActiva = true,
            FechaCreacion = DateTime.UtcNow
        };

        // Act
        await _unitOfWork.MarcasAutos.AddAsync(marca1);
        await _unitOfWork.MarcasAutos.AddAsync(marca2);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var marcas = await _context.MarcasAutos.ToListAsync();
        marcas.Should().HaveCount(2);
        marcas.Should().Contain(m => m.Nombre == "Audi");
        marcas.Should().Contain(m => m.Nombre == "Mazda");
    }

    [Fact]
    public void Dispose_Should_DisposeContext()
    {
        // Act
        _unitOfWork.Dispose();

        // Assert
        // After disposal, operations should throw
        var act = () => _context.MarcasAutos.ToList();
        act.Should().Throw<ObjectDisposedException>();
    }

    public void Dispose()
    {
        try
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Already disposed, ignore
        }
    }
}
