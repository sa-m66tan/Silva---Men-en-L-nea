namespace SilvaAPI.Models;

public class VwReporteAuditoria
{
    public int IdAuditoria { get; set; }
    public int? IdPlatillo { get; set; }
    public string Usuario { get; set; } = null!;
    public string? Rol { get; set; }
    public string Accion { get; set; } = null!;
    public string DetalleCambio { get; set; } = null!;
    public DateTime FechaRegistro { get; set; }
}