namespace SilvaAPI.Models;

public class Usuarios
{
    public int IdUsuario { get; set; }
    public int IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string Contraseña { get; set; } = null!;
    public bool Estado { get; set; } = true;
    public DateTime FechaRegistro { get; set; }

    public virtual Roles Roles { get; set; } = null!;
    public virtual ICollection<Platillos> PlatillosModificados { get; set; } = new List<Platillos>();
    public virtual ICollection<AuditoriaCatalogos> Auditorias { get; set; } = new List<AuditoriaCatalogos>();
}