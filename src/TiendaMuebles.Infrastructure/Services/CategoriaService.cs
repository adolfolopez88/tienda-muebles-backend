using Microsoft.EntityFrameworkCore;
using TiendaMuebles.Application.DTOs;
using TiendaMuebles.Application.Interfaces;
using TiendaMuebles.Domain.Entities;
using TiendaMuebles.Infrastructure.Data;

namespace TiendaMuebles.Infrastructure.Services;

public class CategoriaService : ICategoriaService
{
    private readonly AppDbContext _db;

    public CategoriaService(AppDbContext db) => _db = db;

    public async Task<List<CategoriaResponse>> GetAllAsync()
    {
        return await _db.Categorias
            .OrderBy(c => c.Orden)
            .Select(c => new CategoriaResponse(c.Id.ToString(), c.Nombre, c.Slug, c.Orden))
            .ToListAsync();
    }

    public async Task<CategoriaResponse> CreateAsync(CreateCategoriaRequest r)
    {
        var cat = new Categoria
        {
            Id = Guid.NewGuid(),
            Nombre = r.Nombre,
            Slug = r.Slug,
            Orden = r.Orden
        };
        _db.Categorias.Add(cat);
        await _db.SaveChangesAsync();
        return new CategoriaResponse(cat.Id.ToString(), cat.Nombre, cat.Slug, cat.Orden);
    }
}
