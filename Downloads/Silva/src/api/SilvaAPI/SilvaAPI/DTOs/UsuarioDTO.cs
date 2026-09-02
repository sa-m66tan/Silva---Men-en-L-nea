namespace SilvaAPI.DTOs;

public class UsuarioReadDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public bool Estado { get; set; }
    public string RolNombre { get; set; } = null!;
    public DateTime FechaRegistro { get; set; }
}

public class UsuarioCreateDto
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string Contraseña { get; set; } = null!;
}

public class UsuarioUpdateDto
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public bool Estado { get; set; }
}