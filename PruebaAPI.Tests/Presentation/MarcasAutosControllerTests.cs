using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PruebaAPI.Application.DTOs;
using PruebaAPI.Application.Interfaces;
using PruebaAPI.Presentation.Controllers;

namespace PruebaAPI.Tests.Presentation;

public class MarcasAutosControllerTests
{
    private readonly Mock<IMarcaAutoService> _serviceMock;
    private readonly Mock<ILogger<MarcasAutosController>> _loggerMock;
    private readonly MarcasAutosController _controller;

    public MarcasAutosControllerTests()
    {
        _serviceMock = new Mock<IMarcaAutoService>();
        _loggerMock = new Mock<ILogger<MarcasAutosController>>();
        _controller = new MarcasAutosController(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetMarcas_Should_ReturnOkWithAllMarcas()
    {
        // Arrange
        var marcas = new List<MarcaAutoDto>
        {
            new MarcaAutoDto { Id = 1, Nombre = "Toyota", PaisOrigen = "Japón", AñoFundacion = 1937, EsActiva = true },
            new MarcaAutoDto { Id = 2, Nombre = "Ford", PaisOrigen = "Estados Unidos", AñoFundacion = 1903, EsActiva = true }
        };

        _serviceMock.Setup(s => s.GetAllMarcasAsync()).ReturnsAsync(marcas);

        // Act
        var result = await _controller.GetMarcas();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedMarcas = okResult.Value.Should().BeAssignableTo<IEnumerable<MarcaAutoDto>>().Subject;
        returnedMarcas.Should().HaveCount(2);
        _serviceMock.Verify(s => s.GetAllMarcasAsync(), Times.Once);
    }

    [Fact]
    public async Task GetMarca_Should_ReturnOk_WhenMarcaExists()
    {
        // Arrange
        var marca = new MarcaAutoDto 
        { 
            Id = 1, 
            Nombre = "BMW", 
            PaisOrigen = "Alemania", 
            AñoFundacion = 1916, 
            EsActiva = true 
        };

        _serviceMock.Setup(s => s.GetMarcaByIdAsync(1)).ReturnsAsync(marca);

        // Act
        var result = await _controller.GetMarca(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedMarca = okResult.Value.Should().BeAssignableTo<MarcaAutoDto>().Subject;
        returnedMarca.Nombre.Should().Be("BMW");
        _serviceMock.Verify(s => s.GetMarcaByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetMarca_Should_ReturnNotFound_WhenMarcaDoesNotExist()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetMarcaByIdAsync(999)).ReturnsAsync((MarcaAutoDto?)null);

        // Act
        var result = await _controller.GetMarca(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        _serviceMock.Verify(s => s.GetMarcaByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task BuscarMarcas_Should_ReturnOk_WithMatchingMarcas()
    {
        // Arrange
        var marcas = new List<MarcaAutoDto>
        {
            new MarcaAutoDto { Id = 1, Nombre = "BMW", PaisOrigen = "Alemania" }
        };

        _serviceMock.Setup(s => s.SearchMarcasByNombreAsync("BMW")).ReturnsAsync(marcas);

        // Act
        var result = await _controller.BuscarMarcas("BMW");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedMarcas = okResult.Value.Should().BeAssignableTo<IEnumerable<MarcaAutoDto>>().Subject;
        returnedMarcas.Should().HaveCount(1);
        _serviceMock.Verify(s => s.SearchMarcasByNombreAsync("BMW"), Times.Once);
    }

    [Fact]
    public async Task BuscarMarcas_Should_ReturnBadRequest_WhenNombreIsEmpty()
    {
        // Act
        var result = await _controller.BuscarMarcas("");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _serviceMock.Verify(s => s.SearchMarcasByNombreAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetMarcasActivas_Should_ReturnOk_WithActiveMarcas()
    {
        // Arrange
        var marcas = new List<MarcaAutoDto>
        {
            new MarcaAutoDto { Id = 1, Nombre = "Toyota", EsActiva = true },
            new MarcaAutoDto { Id = 2, Nombre = "Honda", EsActiva = true }
        };

        _serviceMock.Setup(s => s.GetMarcasActivasAsync()).ReturnsAsync(marcas);

        // Act
        var result = await _controller.GetMarcasActivas();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedMarcas = okResult.Value.Should().BeAssignableTo<IEnumerable<MarcaAutoDto>>().Subject;
        returnedMarcas.Should().HaveCount(2);
        returnedMarcas.Should().AllSatisfy(m => m.EsActiva.Should().BeTrue());
        _serviceMock.Verify(s => s.GetMarcasActivasAsync(), Times.Once);
    }

    [Fact]
    public async Task GetMarcasByPais_Should_ReturnOk_WithMarcasFromCountry()
    {
        // Arrange
        var marcas = new List<MarcaAutoDto>
        {
            new MarcaAutoDto { Id = 1, Nombre = "Toyota", PaisOrigen = "Japón" },
            new MarcaAutoDto { Id = 2, Nombre = "Honda", PaisOrigen = "Japón" }
        };

        _serviceMock.Setup(s => s.GetMarcasByPaisOrigenAsync("Japón")).ReturnsAsync(marcas);

        // Act
        var result = await _controller.GetMarcasByPais("Japón");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedMarcas = okResult.Value.Should().BeAssignableTo<IEnumerable<MarcaAutoDto>>().Subject;
        returnedMarcas.Should().HaveCount(2);
        _serviceMock.Verify(s => s.GetMarcasByPaisOrigenAsync("Japón"), Times.Once);
    }

    [Fact]
    public async Task GetMarcasByAñoFundacion_Should_ReturnOk_WithMarcasInRange()
    {
        // Arrange
        var marcas = new List<MarcaAutoDto>
        {
            new MarcaAutoDto { Id = 1, Nombre = "Ford", AñoFundacion = 1903 },
            new MarcaAutoDto { Id = 2, Nombre = "BMW", AñoFundacion = 1916 }
        };

        _serviceMock.Setup(s => s.GetMarcasByAñoFundacionRangeAsync(1900, 1920)).ReturnsAsync(marcas);

        // Act
        var result = await _controller.GetMarcasByAñoFundacion(1900, 1920);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedMarcas = okResult.Value.Should().BeAssignableTo<IEnumerable<MarcaAutoDto>>().Subject;
        returnedMarcas.Should().HaveCount(2);
        _serviceMock.Verify(s => s.GetMarcasByAñoFundacionRangeAsync(1900, 1920), Times.Once);
    }

    [Fact]
    public async Task GetMarcasByAñoFundacion_Should_ReturnBadRequest_WhenRangeIsInvalid()
    {
        // Act
        var result = await _controller.GetMarcasByAñoFundacion(2030, 1900);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _serviceMock.Verify(s => s.GetMarcasByAñoFundacionRangeAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CreateMarca_Should_ReturnCreatedAtAction_WithNewMarca()
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

        var createdMarca = new MarcaAutoDto
        {
            Id = 1,
            Nombre = "Tesla",
            PaisOrigen = "Estados Unidos",
            AñoFundacion = 2003,
            SitioWeb = "https://www.tesla.com",
            EsActiva = true
        };

        _serviceMock.Setup(s => s.CreateMarcaAsync(createDto)).ReturnsAsync(createdMarca);

        // Act
        var result = await _controller.CreateMarca(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(MarcasAutosController.GetMarca));
        var returnedMarca = createdResult.Value.Should().BeAssignableTo<MarcaAutoDto>().Subject;
        returnedMarca.Nombre.Should().Be("Tesla");
        _serviceMock.Verify(s => s.CreateMarcaAsync(createDto), Times.Once);
    }

    [Fact]
    public async Task UpdateMarca_Should_ReturnNoContent_WhenUpdateSucceeds()
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

        _serviceMock.Setup(s => s.UpdateMarcaAsync(1, updateDto)).ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateMarca(1, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _serviceMock.Verify(s => s.UpdateMarcaAsync(1, updateDto), Times.Once);
    }

    [Fact]
    public async Task UpdateMarca_Should_ReturnBadRequest_WhenIdMismatch()
    {
        // Arrange
        var updateDto = new UpdateMarcaAutoDto
        {
            Id = 2,
            Nombre = "Tesla",
            PaisOrigen = "Estados Unidos",
            AñoFundacion = 2003,
            EsActiva = true
        };

        // Act
        var result = await _controller.UpdateMarca(1, updateDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _serviceMock.Verify(s => s.UpdateMarcaAsync(It.IsAny<int>(), It.IsAny<UpdateMarcaAutoDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateMarca_Should_ReturnNotFound_WhenMarcaDoesNotExist()
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

        _serviceMock.Setup(s => s.UpdateMarcaAsync(999, updateDto)).ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateMarca(999, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        _serviceMock.Verify(s => s.UpdateMarcaAsync(999, updateDto), Times.Once);
    }

    [Fact]
    public async Task DeleteMarca_Should_ReturnNoContent_WhenDeleteSucceeds()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteMarcaAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteMarca(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _serviceMock.Verify(s => s.DeleteMarcaAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteMarca_Should_ReturnNotFound_WhenMarcaDoesNotExist()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteMarcaAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteMarca(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        _serviceMock.Verify(s => s.DeleteMarcaAsync(999), Times.Once);
    }
}
