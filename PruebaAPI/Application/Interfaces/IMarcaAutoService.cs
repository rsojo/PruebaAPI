using PruebaAPI.Application.DTOs;

namespace PruebaAPI.Application.Interfaces;

public interface IMarcaAutoService
{
    Task<IEnumerable<MarcaAutoDto>> GetAllMarcasAsync();
    Task<MarcaAutoDto?> GetMarcaByIdAsync(int id);
    Task<MarcaAutoDto> CreateMarcaAsync(CreateMarcaAutoDto dto);
    Task<bool> UpdateMarcaAsync(int id, UpdateMarcaAutoDto dto);
    Task<bool> DeleteMarcaAsync(int id);
    Task<IEnumerable<MarcaAutoDto>> SearchMarcasByNombreAsync(string nombre);
    Task<IEnumerable<MarcaAutoDto>> GetMarcasActivasAsync();
    Task<IEnumerable<MarcaAutoDto>> GetMarcasByPaisOrigenAsync(string pais);
    Task<IEnumerable<MarcaAutoDto>> GetMarcasByAñoFundacionRangeAsync(int añoInicio, int añoFin);
}
