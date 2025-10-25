namespace PruebaAPI.Domain.Entities;

public class MarcaAuto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? PaisOrigen { get; set; }
    public int AñoFundacion { get; set; }
    public string? SitioWeb { get; set; }
    public bool EsActiva { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
