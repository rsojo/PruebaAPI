using FluentAssertions;
using Moq;
using PruebaAPI.Application.DTOs;
using PruebaAPI.Application.Services;
using PruebaAPI.Domain.Entities;
using PruebaAPI.Domain.Interfaces;

namespace PruebaAPI.Tests.Application;

public class MarcaAutoServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMarcaAutoRepository> _marcaAutoRepositoryMock;
    private readonly MarcaAutoService _service;

    public MarcaAutoServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _marcaAutoRepositoryMock = new Mock<IMarcaAutoRepository>();
        
        _unitOfWorkMock.Setup(u => u.MarcasAutos).Returns(_marcaAutoRepositoryMock.Object);
        
        _service = new MarcaAutoService(_unitOfWorkMock.Object, _marcaAutoRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllMarcasAsync_Should_ReturnAllMarcas()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Id = 1, Nombre = "Toyota", PaisOrigen = "Japón", AñoFundacion = 1937, EsActiva = true },
            new MarcaAuto { Id = 2, Nombre = "Ford", PaisOrigen = "Estados Unidos", AñoFundacion = 1903, EsActiva = true }
        };

        _marcaAutoRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(marcas);

        // Act
        var result = await _service.GetAllMarcasAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<MarcaAutoDto>();
        _marcaAutoRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetMarcaByIdAsync_Should_ReturnMarca_WhenExists()
    {
        // Arrange
        var marca = new MarcaAuto 
        { 
            Id = 1, 
            Nombre = "BMW", 
            PaisOrigen = "Alemania", 
            AñoFundacion = 1916, 
            EsActiva = true 
        };

        _marcaAutoRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(marca);

        // Act
        var result = await _service.GetMarcaByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Nombre.Should().Be("BMW");
        _marcaAutoRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetMarcaByIdAsync_Should_ReturnNull_WhenNotExists()
    {
        // Arrange
        _marcaAutoRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((MarcaAuto?)null);

        // Act
        var result = await _service.GetMarcaByIdAsync(999);

        // Assert
        result.Should().BeNull();
        _marcaAutoRepositoryMock.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task SearchMarcasByNombreAsync_Should_ReturnMatchingMarcas()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Id = 1, Nombre = "BMW", PaisOrigen = "Alemania", AñoFundacion = 1916, EsActiva = true }
        };

        _marcaAutoRepositoryMock.Setup(r => r.SearchByNameAsync("BMW")).ReturnsAsync(marcas);

        // Act
        var result = await _service.SearchMarcasByNombreAsync("BMW");

        // Assert
        result.Should().HaveCount(1);
        result.First().Nombre.Should().Be("BMW");
        _marcaAutoRepositoryMock.Verify(r => r.SearchByNameAsync("BMW"), Times.Once);
    }

    [Fact]
    public async Task GetMarcasActivasAsync_Should_ReturnOnlyActiveMarcas()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Id = 1, Nombre = "Toyota", EsActiva = true },
            new MarcaAuto { Id = 2, Nombre = "Honda", EsActiva = true }
        };

        _marcaAutoRepositoryMock.Setup(r => r.GetMarcasActivasAsync()).ReturnsAsync(marcas);

        // Act
        var result = await _service.GetMarcasActivasAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(m => m.EsActiva.Should().BeTrue());
        _marcaAutoRepositoryMock.Verify(r => r.GetMarcasActivasAsync(), Times.Once);
    }

    [Fact]
    public async Task GetMarcasByPaisOrigenAsync_Should_ReturnMarcasFromCountry()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Id = 1, Nombre = "Toyota", PaisOrigen = "Japón" },
            new MarcaAuto { Id = 2, Nombre = "Honda", PaisOrigen = "Japón" }
        };

        _marcaAutoRepositoryMock.Setup(r => r.GetByPaisOrigenAsync("Japón")).ReturnsAsync(marcas);

        // Act
        var result = await _service.GetMarcasByPaisOrigenAsync("Japón");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(m => m.PaisOrigen.Should().Be("Japón"));
        _marcaAutoRepositoryMock.Verify(r => r.GetByPaisOrigenAsync("Japón"), Times.Once);
    }

    [Fact]
    public async Task GetMarcasByAñoFundacionRangeAsync_Should_ReturnMarcasInRange()
    {
        // Arrange
        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto { Id = 1, Nombre = "Ford", AñoFundacion = 1903 },
            new MarcaAuto { Id = 2, Nombre = "BMW", AñoFundacion = 1916 }
        };

        _marcaAutoRepositoryMock.Setup(r => r.GetByAñoFundacionRangeAsync(1900, 1920)).ReturnsAsync(marcas);

        // Act
        var result = await _service.GetMarcasByAñoFundacionRangeAsync(1900, 1920);

        // Assert
        result.Should().HaveCount(2);
        _marcaAutoRepositoryMock.Verify(r => r.GetByAñoFundacionRangeAsync(1900, 1920), Times.Once);
    }

    [Fact]
    public async Task CreateMarcaAsync_Should_CreateAndReturnMarca()
    {
        // Arrange
        var createDto = new CreateMarcaAutoDto
        {
            Nombre = "Tesla",
            PaisOrigen = "Estados Unidos",
            AñoFundacion = 2003,
            SitioWeb = "https://www.tesla.com",
            EsActiva = true
        };

        var createdMarca = new MarcaAuto
        {
            Id = 1,
            Nombre = createDto.Nombre,
            PaisOrigen = createDto.PaisOrigen,
            AñoFundacion = createDto.AñoFundacion,
            SitioWeb = createDto.SitioWeb,
            EsActiva = createDto.EsActiva,
            FechaCreacion = DateTime.UtcNow
        };

        _marcaAutoRepositoryMock.Setup(r => r.AddAsync(It.IsAny<MarcaAuto>())).ReturnsAsync(createdMarca);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateMarcaAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Nombre.Should().Be("Tesla");
        result.PaisOrigen.Should().Be("Estados Unidos");
        _marcaAutoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<MarcaAuto>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateMarcaAsync_Should_UpdateAndReturnTrue_WhenExists()
    {
        // Arrange
        var updateDto = new UpdateMarcaAutoDto
        {
            Id = 1,
            Nombre = "Tesla Motors",
            PaisOrigen = "Estados Unidos",
            AñoFundacion = 2003,
            SitioWeb = "https://www.tesla.com",
            EsActiva = true
        };

        var existingMarca = new MarcaAuto
        {
            Id = 1,
            Nombre = "Tesla",
            PaisOrigen = "Estados Unidos",
            AñoFundacion = 2003,
            EsActiva = true
        };

        _marcaAutoRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingMarca);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.UpdateMarcaAsync(1, updateDto);

        // Assert
        result.Should().BeTrue();
        existingMarca.Nombre.Should().Be("Tesla Motors");
        _marcaAutoRepositoryMock.Verify(r => r.UpdateAsync(existingMarca), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateMarcaAsync_Should_ReturnFalse_WhenNotExists()
    {
        // Arrange
        var updateDto = new UpdateMarcaAutoDto
        {
            Id = 999,
            Nombre = "NonExistent",
            PaisOrigen = "Unknown",
            AñoFundacion = 2000,
            EsActiva = true
        };

        _marcaAutoRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((MarcaAuto?)null);

        // Act
        var result = await _service.UpdateMarcaAsync(999, updateDto);

        // Assert
        result.Should().BeFalse();
        _marcaAutoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<MarcaAuto>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteMarcaAsync_Should_DeleteAndReturnTrue_WhenExists()
    {
        // Arrange
        var marca = new MarcaAuto
        {
            Id = 1,
            Nombre = "Tesla",
            PaisOrigen = "Estados Unidos",
            AñoFundacion = 2003,
            EsActiva = true
        };

        _marcaAutoRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(marca);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.DeleteMarcaAsync(1);

        // Assert
        result.Should().BeTrue();
        _marcaAutoRepositoryMock.Verify(r => r.DeleteAsync(marca), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteMarcaAsync_Should_ReturnFalse_WhenNotExists()
    {
        // Arrange
        _marcaAutoRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((MarcaAuto?)null);

        // Act
        var result = await _service.DeleteMarcaAsync(999);

        // Assert
        result.Should().BeFalse();
        _marcaAutoRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<MarcaAuto>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
