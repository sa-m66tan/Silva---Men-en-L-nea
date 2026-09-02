namespace SilvaAPI.Models;

public class Categorias
{
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = null!;

    public virtual ICollection<Platillos> Platillos { get; set; } = new List<Platillos>();
}