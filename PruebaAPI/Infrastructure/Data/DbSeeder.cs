using PruebaAPI.Domain.Entities;
using PruebaAPI.Infrastructure.Persistence;

namespace PruebaAPI.Infrastructure.Data;

/// <summary>
/// Database seeder for initial data population.
/// Runs automatically in development environment.
/// In production, data should be inserted manually or via specific scripts.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Inserts initial car brand data into the database.
    /// Only executes if the MarcasAutos table is empty.
    /// </summary>
    /// <param name="context">Database context</param>
    public static async Task SeedAsync(AppDbContext context)
    {
        // Check if data already exists
        if (context.MarcasAutos.Any())
        {
            return; // Data already exists, skip seeding
        }

        var marcas = new List<MarcaAuto>
        {
            new MarcaAuto
            {
                Nombre = "Toyota",
                PaisOrigen = "Japón",
                AñoFundacion = 1937,
                SitioWeb = "https://www.toyota.com",
                EsActiva = true,
                FechaCreacion = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new MarcaAuto
            {
                Nombre = "Ford",
                PaisOrigen = "Estados Unidos",
                AñoFundacion = 1903,
                SitioWeb = "https://www.ford.com",
                EsActiva = true,
                FechaCreacion = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new MarcaAuto
            {
                Nombre = "BMW",
                PaisOrigen = "Alemania",
                AñoFundacion = 1916,
                SitioWeb = "https://www.bmw.com",
                EsActiva = true,
                FechaCreacion = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new MarcaAuto
            {
                Nombre = "Mercedes-Benz",
                PaisOrigen = "Alemania",
                AñoFundacion = 1926,
                SitioWeb = "https://www.mercedes-benz.com",
                EsActiva = true,
                FechaCreacion = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new MarcaAuto
            {
                Nombre = "Ferrari",
                PaisOrigen = "Italia",
                AñoFundacion = 1939,
                SitioWeb = "https://www.ferrari.com",
                EsActiva = true,
                FechaCreacion = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new MarcaAuto
            {
                Nombre = "Tesla",
                PaisOrigen = "Estados Unidos",
                AñoFundacion = 2003,
                SitioWeb = "https://www.tesla.com",
                EsActiva = true,
                FechaCreacion = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new MarcaAuto
            {
                Nombre = "Volkswagen",
                PaisOrigen = "Alemania",
                AñoFundacion = 1937,
                SitioWeb = "https://www.volkswagen.com",
                EsActiva = true,
                FechaCreacion = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new MarcaAuto
            {
                Nombre = "Honda",
                PaisOrigen = "Japón",
                AñoFundacion = 1948,
                SitioWeb = "https://www.honda.com",
                EsActiva = true,
                FechaCreacion = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new MarcaAuto
            {
                Nombre = "Chevrolet",
                PaisOrigen = "Estados Unidos",
                AñoFundacion = 1911,
                SitioWeb = "https://www.chevrolet.com",
                EsActiva = true,
                FechaCreacion = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new MarcaAuto
            {
                Nombre = "Nissan",
                PaisOrigen = "Japón",
                AñoFundacion = 1933,
                SitioWeb = "https://www.nissan.com",
                EsActiva = true,
                FechaCreacion = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        await context.MarcasAutos.AddRangeAsync(marcas);
        await context.SaveChangesAsync();
    }
}
