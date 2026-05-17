using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaMuebles.Application.DTOs;
using TiendaMuebles.Application.Interfaces;

namespace TiendaMuebles.Api.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _svc;

    public ProductosController(IProductoService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<ProductoListResponse>> GetAll(
        [FromQuery] string? categoria,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 12)
    {
        return Ok(await _svc.GetProductosAsync(categoria, page, limit));
    }

    [HttpGet("destacados")]
    public async Task<ActionResult<ProductoListResponse>> GetDestacados()
    {
        return Ok(await _svc.GetDestacadosAsync());
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ProductoResponse>> GetBySlug(string slug)
    {
        var p = await _svc.GetBySlugAsync(slug);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductoResponse>> Create(CreateProductoRequest r)
    {
        var p = await _svc.CreateAsync(r);
        return CreatedAtAction(nameof(GetBySlug), new { slug = p.Slug }, p);
    }

    [HttpPut("{slug}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductoResponse>> Update(string slug, UpdateProductoRequest r)
    {
        var p = await _svc.UpdateAsync(slug, r);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPatch("{slug}/toggle")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Toggle(string slug)
    {
        var ok = await _svc.ToggleAsync(slug);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{slug}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(string slug)
    {
        var ok = await _svc.DeleteAsync(slug);
        return ok ? NoContent() : NotFound();
    }
}
