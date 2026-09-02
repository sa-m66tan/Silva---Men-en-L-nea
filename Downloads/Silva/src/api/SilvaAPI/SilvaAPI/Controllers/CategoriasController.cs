using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SilvaAPI.Data;
using SilvaAPI.DTOs;
using SilvaAPI.Models;

namespace SilvaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly SilvaContext _context;

    public CategoriasController(SilvaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
    {
        return await _context.Categorias
            .Select(c => new CategoriaDto { IdCategoria = c.IdCategoria, Nombre = c.Nombre })
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> CreateCategoria([FromBody] CategoriaCreateDto dto)
    {
        var categoria = new Categorias { Nombre = dto.Nombre };
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return Ok(new CategoriaDto { IdCategoria = categoria.IdCategoria, Nombre = categoria.Nombre });
    }
}