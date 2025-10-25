using PruebaAPI.Domain.Entities;

namespace PruebaAPI.Domain.Interfaces;

public interface IMarcaAutoRepository : IRepository<MarcaAuto>
{
    Task<IEnumerable<MarcaAuto>> SearchByNameAsync(string nombre);
    Task<IEnumerable<MarcaAuto>> GetMarcasActivasAsync();
    Task<IEnumerable<MarcaAuto>> GetByPaisOrigenAsync(string pais);
    Task<IEnumerable<MarcaAuto>> GetByAñoFundacionRangeAsync(int añoInicio, int añoFin);
}
