using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaMuebles.Application.DTOs;
using TiendaMuebles.Application.Interfaces;

namespace TiendaMuebles.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _svc;

    public CategoriasController(ICategoriaService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<List<CategoriaResponse>>> GetAll()
    {
        return Ok(await _svc.GetAllAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoriaResponse>> Create(CreateCategoriaRequest r)
    {
        return Ok(await _svc.CreateAsync(r));
    }
}
