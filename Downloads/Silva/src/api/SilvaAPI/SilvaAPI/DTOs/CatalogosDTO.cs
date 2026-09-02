namespace SilvaAPI.DTOs;

public class CategoriaDto
{
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = null!;
}

public class CategoriaCreateDto
{
    public string Nombre { get; set; } = null!;
}

public class IngredienteDto
{
    public int IdIngrediente { get; set; }
    public string Nombre { get; set; } = null!;
}

public class IngredienteCreateDto
{
    public string Nombre { get; set; } = null!;
}