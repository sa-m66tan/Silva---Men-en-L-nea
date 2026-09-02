using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SilvaAPI.Data;
using SilvaAPI.DTOs;
using SilvaAPI.Models;

namespace SilvaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatillosController : ControllerBase
{
    private readonly SilvaContext _context;

    public PlatillosController(SilvaContext context)
    {
        _context = context;
    }

    [HttpGet("menu-publico")]
    public async Task<ActionResult<IEnumerable<VwMenuPublico>>> GetMenuPublico()
    {
        return await _context.VwMenuPublicos.ToListAsync();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatilloReadDto>>> GetPlatillos()
    {
        return await _context.Platillos
            .Include(p => p.Categorias)
            .Include(p => p.Ingredientes)
            .Select(p => new PlatilloReadDto
            {
                IdPlatillo = p.IdPlatillo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                ImagenUrl = p.ImagenUrl,
                TiempoPreparacion = p.TiempoPreparacion,
                Estado = p.Estado,
                IdCategoria = p.IdCategoria,
                CategoriaNombre = p.Categorias.Nombre,
                Ingredientes = p.Ingredientes.Select(i => i.Nombre).ToList()
            })
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlatilloReadDto>> GetPlatillo(int id)
    {
        var p = await _context.Platillos
            .Include(p => p.Categorias)
            .Include(p => p.Ingredientes)
            .FirstOrDefaultAsync(x => x.IdPlatillo == id);

        if (p == null) return NotFound("El platillo solicitado no existe.");

        var dto = new PlatilloReadDto
        {
            IdPlatillo = p.IdPlatillo,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Precio = p.Precio,
            ImagenUrl = p.ImagenUrl,
            TiempoPreparacion = p.TiempoPreparacion,
            Estado = p.Estado,
            IdCategoria = p.IdCategoria,
            CategoriaNombre = p.Categorias.Nombre,
            Ingredientes = p.Ingredientes.Select(i => i.Nombre).ToList()
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<PlatilloReadDto>> CreatePlatillo([FromBody] PlatilloCreateUpdateDto dto)
    {
        var platillo = new Platillos
        {
            IdCategoria = dto.IdCategoria,
            IdUsuarioUltimaModif = dto.IdUsuarioModif,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            ImagenUrl = dto.ImagenUrl,
            TiempoPreparacion = dto.TiempoPreparacion,
            Estado = dto.Estado
        };

        if (dto.IngredientesIds.Any())
        {
            var ingredientes = await _context.Ingredientes
                .Where(i => dto.IngredientesIds.Contains(i.IdIngrediente))
                .ToListAsync();

            platillo.Ingredientes = ingredientes;
        }

        _context.Platillos.Add(platillo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPlatillo), new { id = platillo.IdPlatillo }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlatillo(int id, [FromBody] PlatilloCreateUpdateDto dto)
    {
        var platillo = await _context.Platillos
            .Include(p => p.Ingredientes)
            .FirstOrDefaultAsync(p => p.IdPlatillo == id);

        if (platillo == null) return NotFound("Platillo no encontrado.");

        platillo.IdCategoria = dto.IdCategoria;
        platillo.IdUsuarioUltimaModif = dto.IdUsuarioModif;
        platillo.Nombre = dto.Nombre;
        platillo.Descripcion = dto.Descripcion;
        platillo.Precio = dto.Precio;
        platillo.ImagenUrl = dto.ImagenUrl;
        platillo.TiempoPreparacion = dto.TiempoPreparacion;
        platillo.Estado = dto.Estado;

        platillo.Ingredientes.Clear();
        if (dto.IngredientesIds.Any())
        {
            var ingredientes = await _context.Ingredientes
                .Where(i => dto.IngredientesIds.Contains(i.IdIngrediente))
                .ToListAsync();

            platillo.Ingredientes = ingredientes;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoDto dto)
    {
        var platillo = await _context.Platillos.FindAsync(id);
        if (platillo == null) return NotFound("Platillo no encontrado.");

        platillo.Estado = dto.NuevoEstado;
        platillo.IdUsuarioUltimaModif = dto.IdUsuarioModif;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlatillo(int id)
    {
        var platillo = await _context.Platillos.FindAsync(id);
        if (platillo == null) return NotFound("Platillo no encontrado.");

        _context.Platillos.Remove(platillo);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}