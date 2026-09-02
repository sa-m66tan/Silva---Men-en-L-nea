namespace SilvaAPI.Models;

public class Roles
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
}