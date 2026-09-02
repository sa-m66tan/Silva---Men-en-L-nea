namespace SilvaAPI.Models;

public class Ingredientes
{
    public int IdIngrediente { get; set; }
    public string Nombre { get; set; } = null!;

    public virtual ICollection<Platillos> Platillos { get; set; } = new List<Platillos>();
}