using Microsoft.AspNetCore.Mvc;
using PruebaAPI.Application.DTOs;
using PruebaAPI.Application.Interfaces;

namespace PruebaAPI.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarcasAutosController : ControllerBase
{
    private readonly IMarcaAutoService _marcaAutoService;
    private readonly ILogger<MarcasAutosController> _logger;

    public MarcasAutosController(IMarcaAutoService marcaAutoService, ILogger<MarcasAutosController> logger)
    {
        _marcaAutoService = marcaAutoService;
        _logger = logger;
    }

    /// <summary>
    /// Get all car brands
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MarcaAutoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MarcaAutoDto>>> GetMarcas()
    {
        _logger.LogInformation("API: Getting all car brands");
        var marcas = await _marcaAutoService.GetAllMarcasAsync();
        return Ok(marcas);
    }

    /// <summary>
    /// Get car brand by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MarcaAutoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MarcaAutoDto>> GetMarca(int id)
    {
        _logger.LogInformation("API: Getting brand with id: {Id}", id);
        var marca = await _marcaAutoService.GetMarcaByIdAsync(id);

        if (marca == null)
        {
            return NotFound(new { message = $"Brand with ID {id} not found" });
        }

        return Ok(marca);
    }

    /// <summary>
    /// Search brands by name
    /// </summary>
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(IEnumerable<MarcaAutoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<MarcaAutoDto>>> BuscarMarcas([FromQuery] string nombre)
    {
        _logger.LogInformation("API: Searching brands by name: {Nombre}", nombre);
        
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return BadRequest(new { message = "Search name cannot be empty" });
        }

        var marcas = await _marcaAutoService.SearchMarcasByNombreAsync(nombre);
        return Ok(marcas);
    }

    /// <summary>
    /// Get active brands
    /// </summary>
    [HttpGet("activas")]
    [ProducesResponseType(typeof(IEnumerable<MarcaAutoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MarcaAutoDto>>> GetMarcasActivas()
    {
        _logger.LogInformation("API: Getting active brands");
        var marcas = await _marcaAutoService.GetMarcasActivasAsync();
        return Ok(marcas);
    }

    /// <summary>
    /// Get brands by country of origin
    /// </summary>
    [HttpGet("pais/{pais}")]
    [ProducesResponseType(typeof(IEnumerable<MarcaAutoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MarcaAutoDto>>> GetMarcasByPais(string pais)
    {
        _logger.LogInformation("API: Getting brands from country: {Pais}", pais);
        var marcas = await _marcaAutoService.GetMarcasByPaisOrigenAsync(pais);
        return Ok(marcas);
    }

    /// <summary>
    /// Get brands by foundation year range
    /// </summary>
    [HttpGet("anio-fundacion")]
    [ProducesResponseType(typeof(IEnumerable<MarcaAutoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<MarcaAutoDto>>> GetMarcasByAñoFundacion(
        [FromQuery] int añoInicio,
        [FromQuery] int añoFin)
    {
        _logger.LogInformation("API: Getting brands founded between {AñoInicio} and {AñoFin}", añoInicio, añoFin);

        if (añoInicio < 1800 || añoFin > DateTime.Now.Year || añoInicio > añoFin)
        {
            return BadRequest(new { message = "Invalid year range" });
        }

        var marcas = await _marcaAutoService.GetMarcasByAñoFundacionRangeAsync(añoInicio, añoFin);
        return Ok(marcas);
    }

    /// <summary>
    /// Create a new car brand
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MarcaAutoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MarcaAutoDto>> CreateMarca([FromBody] CreateMarcaAutoDto createMarcaDto)
    {
        _logger.LogInformation("API: Creating new brand: {Nombre}", createMarcaDto.Nombre);
        
        var marca = await _marcaAutoService.CreateMarcaAsync(createMarcaDto);
        return CreatedAtAction(nameof(GetMarca), new { id = marca.Id }, marca);
    }

    /// <summary>
    /// Update an existing car brand
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMarca(int id, [FromBody] UpdateMarcaAutoDto updateMarcaDto)
    {
        if (id != updateMarcaDto.Id)
        {
            return BadRequest(new { message = "Brand ID mismatch" });
        }

        _logger.LogInformation("API: Updating brand with id: {Id}", id);

        var result = await _marcaAutoService.UpdateMarcaAsync(id, updateMarcaDto);
        if (!result)
        {
            return NotFound(new { message = $"Brand with ID {id} not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Delete a car brand
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMarca(int id)
    {
        _logger.LogInformation("API: Deleting brand with id: {Id}", id);
        
        var result = await _marcaAutoService.DeleteMarcaAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Brand with ID {id} not found" });
        }

        return NoContent();
    }
}
