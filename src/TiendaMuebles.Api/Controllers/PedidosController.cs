using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaMuebles.Application.DTOs;
using TiendaMuebles.Application.Interfaces;

namespace TiendaMuebles.Api.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _svc;

    public PedidosController(IPedidoService svc) => _svc = svc;

    [HttpPost]
    public async Task<ActionResult<PedidoResponse>> Create(CreatePedidoRequest r)
    {
        var p = await _svc.CreateAsync(r);
        return CreatedAtAction(nameof(GetByNumero), new { numero = p.Numero }, p);
    }

    [HttpGet("{numero}")]
    public async Task<ActionResult<PedidoResponse>> GetByNumero(string numero)
    {
        var p = await _svc.GetByNumeroAsync(numero);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PedidoListResponse>> GetAll(
        [FromQuery] string? estado,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 12)
    {
        return Ok(await _svc.GetAllAsync(estado, page, limit));
    }

    [HttpPatch("{numero}/estado")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PedidoResponse>> UpdateEstado(string numero, UpdateEstadoRequest r)
    {
        var p = await _svc.UpdateEstadoAsync(numero, r.Estado);
        return p is null ? NotFound() : Ok(p);
    }
}
