using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SilvaAPI.Data;
using SilvaAPI.Models;

namespace SilvaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuditoriaController : ControllerBase
{
    private readonly SilvaContext _context;

    public AuditoriaController(SilvaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VwReporteAuditoria>>> GetReporteAuditoria()
    {
        return await _context.VwReporteAuditorias
            .OrderByDescending(a => a.FechaRegistro)
            .ToListAsync();
    }
}