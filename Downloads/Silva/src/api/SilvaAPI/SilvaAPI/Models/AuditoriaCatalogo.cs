namespace SilvaAPI.Models;

public class AuditoriaCatalogos
{
    public int IdAuditoria { get; set; }
    public string Accion { get; set; } = null!;
    public string DetalleCambio { get; set; } = null!;
    public DateTime FechaRegistro { get; set; }

    public int? IdUsuarioModif { get; set; }
    public virtual Usuarios? UsuarioModif { get; set; }

    public int? IdPlatillo { get; set; }
    public virtual Platillos? Platillos { get; set; }
}