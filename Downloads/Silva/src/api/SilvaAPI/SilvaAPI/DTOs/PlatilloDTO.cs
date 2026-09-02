namespace SilvaAPI.DTOs;

public class PlatilloReadDto
{
    public int IdPlatillo { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public int? TiempoPreparacion { get; set; }
    public string Estado { get; set; } = null!;
    public int IdCategoria { get; set; }
    public string CategoriaNombre { get; set; } = null!;
    public List<string> Ingredientes { get; set; } = new();
}

public class PlatilloCreateUpdateDto
{
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public int? TiempoPreparacion { get; set; }
    public string Estado { get; set; } = "Disponible";
    public int IdUsuarioModif { get; set; }
    public List<int> IngredientesIds { get; set; } = new();
}

public class CambiarEstadoDto
{
    public string NuevoEstado { get; set; } = null!;
    public int IdUsuarioModif { get; set; }
}