namespace SilvaAPI.Models;

public class VwMenuPublico
{
    public int IdPlatillo { get; set; }
    public string Platillo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public int? TiempoPreparacion { get; set; }
    public string Estado { get; set; } = null!;
    public string Categoria { get; set; } = null!;
}