using Microsoft.EntityFrameworkCore;
using PruebaAPI.Domain.Entities;
using PruebaAPI.Domain.Interfaces;
using PruebaAPI.Infrastructure.Persistence;

namespace PruebaAPI.Infrastructure.Repositories;

public class MarcaAutoRepository : Repository<MarcaAuto>, IMarcaAutoRepository
{
    public MarcaAutoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MarcaAuto>> SearchByNameAsync(string nombre)
    {
        return await _context.Set<MarcaAuto>()
            .Where(m => m.Nombre.ToLower().Contains(nombre.ToLower()))
            .ToListAsync();
    }

    public async Task<IEnumerable<MarcaAuto>> GetMarcasActivasAsync()
    {
        return await _context.Set<MarcaAuto>()
            .Where(m => m.EsActiva)
            .ToListAsync();
    }

    public async Task<IEnumerable<MarcaAuto>> GetByPaisOrigenAsync(string pais)
    {
        return await _context.Set<MarcaAuto>()
            .Where(m => m.PaisOrigen != null && m.PaisOrigen.ToLower() == pais.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<MarcaAuto>> GetByAñoFundacionRangeAsync(int añoInicio, int añoFin)
    {
        return await _context.Set<MarcaAuto>()
            .Where(m => m.AñoFundacion >= añoInicio && m.AñoFundacion <= añoFin)
            .OrderBy(m => m.AñoFundacion)
            .ToListAsync();
    }
}
