using System.ComponentModel.DataAnnotations.Schema;

namespace SilvaAPI.Models;

public class Platillos
{
    public int IdPlatillo { get; set; }
    public int IdCategoria { get; set; }
    public int? IdUsuarioUltimaModif { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string? ImagenUrl { get; set; }
    public int? TiempoPreparacion { get; set; }
    public string Estado { get; set; } = "Disponible";

    public virtual Categorias Categorias { get; set; } = null!;

    [ForeignKey("IdUsuarioUltimaModif")]
    public virtual Usuarios? UsuarioUltimaModif { get; set; }
    public virtual ICollection<AuditoriaCatalogos> Auditorias { get; set; } = new List<AuditoriaCatalogos>();
    public virtual ICollection<Ingredientes> Ingredientes { get; set; } = new List<Ingredientes>();
}