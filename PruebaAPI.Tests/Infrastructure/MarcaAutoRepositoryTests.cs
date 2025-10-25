using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PruebaAPI.Domain.Entities;
using PruebaAPI.Infrastructure.Persistence;
using PruebaAPI.Infrastructure.Repositories;

namespace PruebaAPI.Tests.Infrastructure;

public class MarcaAutoRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly MarcaAutoRepository _repository;

    public MarcaAutoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new MarcaAutoRepository(_context);
    }

    [Fact]
    public async Task SearchByNameAsync_Should_ReturnMatchingMarcas_CaseInsensitive()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Nombre = "BMW", PaisOrigen = "Alemania", AñoFundacion = 1916, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Mercedes-Benz", PaisOrigen = "Alemania", AñoFundacion = 1926, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Toyota", PaisOrigen = "Japón", AñoFundacion = 1937, EsActiva = true, FechaCreacion = DateTime.UtcNow }
        };

        await _context.MarcasAutos.AddRangeAsync(marcas);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchByNameAsync("bmw");

        // Assert
        result.Should().HaveCount(1);
        result.First().Nombre.Should().Be("BMW");
    }

    [Fact]
    public async Task SearchByNameAsync_Should_ReturnPartialMatches()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Nombre = "Mercedes-Benz", PaisOrigen = "Alemania", AñoFundacion = 1926, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Benz Motors", PaisOrigen = "Alemania", AñoFundacion = 1900, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Toyota", PaisOrigen = "Japón", AñoFundacion = 1937, EsActiva = true, FechaCreacion = DateTime.UtcNow }
        };

        await _context.MarcasAutos.AddRangeAsync(marcas);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchByNameAsync("benz");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(m => m.Nombre == "Mercedes-Benz");
        result.Should().Contain(m => m.Nombre == "Benz Motors");
    }

    [Fact]
    public async Task GetMarcasActivasAsync_Should_ReturnOnlyActiveMarcas()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Nombre = "Toyota", PaisOrigen = "Japón", AñoFundacion = 1937, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Ford", PaisOrigen = "Estados Unidos", AñoFundacion = 1903, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Oldsmobile", PaisOrigen = "Estados Unidos", AñoFundacion = 1897, EsActiva = false, FechaCreacion = DateTime.UtcNow }
        };

        await _context.MarcasAutos.AddRangeAsync(marcas);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetMarcasActivasAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(m => m.EsActiva.Should().BeTrue());
        result.Should().NotContain(m => m.Nombre == "Oldsmobile");
    }

    [Fact]
    public async Task GetByPaisOrigenAsync_Should_ReturnMarcasFromSpecificCountry()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Nombre = "Toyota", PaisOrigen = "Japón", AñoFundacion = 1937, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Honda", PaisOrigen = "Japón", AñoFundacion = 1948, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "BMW", PaisOrigen = "Alemania", AñoFundacion = 1916, EsActiva = true, FechaCreacion = DateTime.UtcNow }
        };

        await _context.MarcasAutos.AddRangeAsync(marcas);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByPaisOrigenAsync("Japón");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(m => m.PaisOrigen.Should().Be("Japón"));
    }

    [Fact]
    public async Task GetByPaisOrigenAsync_Should_BeCaseInsensitive()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Nombre = "Toyota", PaisOrigen = "Japón", AñoFundacion = 1937, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Honda", PaisOrigen = "Japón", AñoFundacion = 1948, EsActiva = true, FechaCreacion = DateTime.UtcNow }
        };

        await _context.MarcasAutos.AddRangeAsync(marcas);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByPaisOrigenAsync("japón");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByPaisOrigenAsync_Should_HandleNullPaisOrigen()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Nombre = "Unknown Brand", PaisOrigen = null, AñoFundacion = 2000, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Toyota", PaisOrigen = "Japón", AñoFundacion = 1937, EsActiva = true, FechaCreacion = DateTime.UtcNow }
        };

        await _context.MarcasAutos.AddRangeAsync(marcas);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByPaisOrigenAsync("Japón");

        // Assert
        result.Should().HaveCount(1);
        result.First().Nombre.Should().Be("Toyota");
    }

    [Fact]
    public async Task GetByAñoFundacionRangeAsync_Should_ReturnMarcasInRange()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Nombre = "Ford", PaisOrigen = "Estados Unidos", AñoFundacion = 1903, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "BMW", PaisOrigen = "Alemania", AñoFundacion = 1916, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Toyota", PaisOrigen = "Japón", AñoFundacion = 1937, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Tesla", PaisOrigen = "Estados Unidos", AñoFundacion = 2003, EsActiva = true, FechaCreacion = DateTime.UtcNow }
        };

        await _context.MarcasAutos.AddRangeAsync(marcas);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAñoFundacionRangeAsync(1900, 1940);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(m => m.Nombre == "Ford");
        result.Should().Contain(m => m.Nombre == "BMW");
        result.Should().Contain(m => m.Nombre == "Toyota");
        result.Should().NotContain(m => m.Nombre == "Tesla");
    }

    [Fact]
    public async Task GetByAñoFundacionRangeAsync_Should_ReturnOrderedByAñoFundacion()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Nombre = "Toyota", PaisOrigen = "Japón", AñoFundacion = 1937, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Ford", PaisOrigen = "Estados Unidos", AñoFundacion = 1903, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "BMW", PaisOrigen = "Alemania", AñoFundacion = 1916, EsActiva = true, FechaCreacion = DateTime.UtcNow }
        };

        await _context.MarcasAutos.AddRangeAsync(marcas);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAñoFundacionRangeAsync(1900, 1940);

        // Assert
        var resultList = result.ToList();
        resultList.Should().HaveCount(3);
        resultList[0].Nombre.Should().Be("Ford");    // 1903
        resultList[1].Nombre.Should().Be("BMW");     // 1916
        resultList[2].Nombre.Should().Be("Toyota");  // 1937
    }

    [Fact]
    public async Task GetByAñoFundacionRangeAsync_Should_ReturnEmpty_WhenNoMarcasInRange()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Nombre = "Ford", PaisOrigen = "Estados Unidos", AñoFundacion = 1903, EsActiva = true, FechaCreacion = DateTime.UtcNow },
            new MarcaAuto { Nombre = "Tesla", PaisOrigen = "Estados Unidos", AñoFundacion = 2003, EsActiva = true, FechaCreacion = DateTime.UtcNow }
        };

        await _context.MarcasAutos.AddRangeAsync(marcas);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAñoFundacionRangeAsync(1950, 1990);

        // Assert
        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
