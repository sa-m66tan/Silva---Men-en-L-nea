using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SilvaAPI.Data;
using SilvaAPI.DTOs;
using SilvaAPI.Models;

namespace SilvaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly SilvaContext _context;

    public UsuariosController(SilvaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioReadDto>>> GetUsuarios()
    {
        return await _context.Usuarios
            .Include(u => u.Roles)
            .Select(u => new UsuarioReadDto
            {
                IdUsuario = u.IdUsuario,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Correo = u.Correo,
                Estado = u.Estado,
                RolNombre = u.Roles.Nombre,
                FechaRegistro = u.FechaRegistro
            })
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioReadDto>> GetUsuario(int id)
    {
        var u = await _context.Usuarios
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(x => x.IdUsuario == id);

        if (u == null) return NotFound("Usuario no encontrado.");

        return Ok(new UsuarioReadDto
        {
            IdUsuario = u.IdUsuario,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Correo = u.Correo,
            Estado = u.Estado,
            RolNombre = u.Roles.Nombre,
            FechaRegistro = u.FechaRegistro
        });
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioReadDto>> CreateUsuario([FromBody] UsuarioCreateDto dto)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
        {
            return BadRequest("El correo electrónico ya está registrado.");
        }

        var usuario = new Usuarios
        {
            IdRol = dto.IdRol,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Correo = dto.Correo,
            Contraseña = dto.Contraseña
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, new UsuarioReadDto
        {
            IdUsuario = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Correo = usuario.Correo,
            Estado = usuario.Estado,
            FechaRegistro = usuario.FechaRegistro
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioUpdateDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound("Usuario no encontrado.");

        usuario.IdRol = dto.IdRol;
        usuario.Nombre = dto.Nombre;
        usuario.Apellido = dto.Apellido;
        usuario.Correo = dto.Correo;
        usuario.Estado = dto.Estado;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}