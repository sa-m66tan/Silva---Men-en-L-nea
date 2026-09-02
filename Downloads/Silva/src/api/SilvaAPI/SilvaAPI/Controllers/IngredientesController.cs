using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SilvaAPI.Data;
using SilvaAPI.DTOs;
using SilvaAPI.Models;

namespace SilvaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IngredientesController : ControllerBase
{
    private readonly SilvaContext _context;

    public IngredientesController(SilvaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredienteDto>>> GetIngredientes()
    {
        return await _context.Ingredientes
            .Select(i => new IngredienteDto { IdIngrediente = i.IdIngrediente, Nombre = i.Nombre })
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<IngredienteDto>> CreateIngrediente([FromBody] IngredienteCreateDto dto)
    {
        var ingrediente = new Ingredientes { Nombre = dto.Nombre };
        _context.Ingredientes.Add(ingrediente);
        await _context.SaveChangesAsync();

        return Ok(new IngredienteDto { IdIngrediente = ingrediente.IdIngrediente, Nombre = ingrediente.Nombre });
    }
}