namespace PruebaAPI.Application.DTOs;

public class CreateMarcaAutoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? PaisOrigen { get; set; }
    public int AñoFundacion { get; set; }
    public string? SitioWeb { get; set; }
    public bool EsActiva { get; set; } = true;
}
