using PruebaAPI.Application.DTOs;
using PruebaAPI.Application.Interfaces;
using PruebaAPI.Domain.Entities;
using PruebaAPI.Domain.Interfaces;

namespace PruebaAPI.Application.Services;

public class MarcaAutoService : IMarcaAutoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMarcaAutoRepository _marcaAutoRepository;

    public MarcaAutoService(IUnitOfWork unitOfWork, IMarcaAutoRepository marcaAutoRepository)
    {
        _unitOfWork = unitOfWork;
        _marcaAutoRepository = marcaAutoRepository;
    }

    public async Task<IEnumerable<MarcaAutoDto>> GetAllMarcasAsync()
    {
        var marcas = await _marcaAutoRepository.GetAllAsync();
        return marcas.Select(MapToDto);
    }

    public async Task<MarcaAutoDto?> GetMarcaByIdAsync(int id)
    {
        var marca = await _marcaAutoRepository.GetByIdAsync(id);
        return marca == null ? null : MapToDto(marca);
    }

    public async Task<MarcaAutoDto> CreateMarcaAsync(CreateMarcaAutoDto dto)
    {
        var marca = new MarcaAuto
        {
            Nombre = dto.Nombre,
            PaisOrigen = dto.PaisOrigen,
            AñoFundacion = dto.AñoFundacion,
            SitioWeb = dto.SitioWeb,
            EsActiva = dto.EsActiva,
            FechaCreacion = DateTime.UtcNow
        };

        await _marcaAutoRepository.AddAsync(marca);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(marca);
    }

    public async Task<bool> UpdateMarcaAsync(int id, UpdateMarcaAutoDto dto)
    {
        var marca = await _marcaAutoRepository.GetByIdAsync(id);
        if (marca == null)
            return false;

        marca.Nombre = dto.Nombre;
        marca.PaisOrigen = dto.PaisOrigen;
        marca.AñoFundacion = dto.AñoFundacion;
        marca.SitioWeb = dto.SitioWeb;
        marca.EsActiva = dto.EsActiva;

        await _marcaAutoRepository.UpdateAsync(marca);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteMarcaAsync(int id)
    {
        var marca = await _marcaAutoRepository.GetByIdAsync(id);
        if (marca == null)
            return false;

        await _marcaAutoRepository.DeleteAsync(marca);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<MarcaAutoDto>> SearchMarcasByNombreAsync(string nombre)
    {
        var marcas = await _marcaAutoRepository.SearchByNameAsync(nombre);
        return marcas.Select(MapToDto);
    }

    public async Task<IEnumerable<MarcaAutoDto>> GetMarcasActivasAsync()
    {
        var marcas = await _marcaAutoRepository.GetMarcasActivasAsync();
        return marcas.Select(MapToDto);
    }

    public async Task<IEnumerable<MarcaAutoDto>> GetMarcasByPaisOrigenAsync(string pais)
    {
        var marcas = await _marcaAutoRepository.GetByPaisOrigenAsync(pais);
        return marcas.Select(MapToDto);
    }

    public async Task<IEnumerable<MarcaAutoDto>> GetMarcasByAñoFundacionRangeAsync(int añoInicio, int añoFin)
    {
        var marcas = await _marcaAutoRepository.GetByAñoFundacionRangeAsync(añoInicio, añoFin);
        return marcas.Select(MapToDto);
    }

    private static MarcaAutoDto MapToDto(MarcaAuto marca)
    {
        return new MarcaAutoDto
        {
            Id = marca.Id,
            Nombre = marca.Nombre,
            PaisOrigen = marca.PaisOrigen,
            AñoFundacion = marca.AñoFundacion,
            SitioWeb = marca.SitioWeb,
            EsActiva = marca.EsActiva,
            FechaCreacion = marca.FechaCreacion
        };
    }
}
