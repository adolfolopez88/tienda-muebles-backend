using Microsoft.EntityFrameworkCore;
using TiendaMuebles.Application.DTOs;
using TiendaMuebles.Application.Interfaces;
using TiendaMuebles.Domain.Entities;
using TiendaMuebles.Infrastructure.Data;

namespace TiendaMuebles.Infrastructure.Services;

public class ProductoService : IProductoService
{
    private readonly AppDbContext _db;

    public ProductoService(AppDbContext db) => _db = db;

    public async Task<ProductoListResponse> GetProductosAsync(string? categoria, int page = 1, int limit = 12)
    {
        var query = _db.Productos
            .Include(p => p.Imagenes)
            .Where(p => p.Activo);

        if (!string.IsNullOrEmpty(categoria) && categoria != "all")
            query = query.Where(p => p.CategoriaSlug == categoria);

        var total = await query.CountAsync();
        var productos = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return new ProductoListResponse(
            productos.Select(Map).ToList(),
            new PaginationInfo(page, limit, total, (int)Math.Ceiling((double)total / limit))
        );
    }

    public async Task<ProductoListResponse> GetDestacadosAsync()
    {
        var productos = await _db.Productos
            .Include(p => p.Imagenes)
            .Where(p => p.Activo && p.Stock > 0 && p.Badge != null)
            .OrderByDescending(p => p.CreatedAt)
            .Take(12)
            .ToListAsync();

        return new ProductoListResponse(
            productos.Select(Map).ToList(),
            new PaginationInfo(1, 12, productos.Count, 1)
        );
    }

    public async Task<ProductoResponse?> GetBySlugAsync(string slug)
    {
        var p = await _db.Productos
            .Include(x => x.Imagenes)
            .FirstOrDefaultAsync(x => x.Slug == slug && x.Activo);

        return p is null ? null : Map(p);
    }

    public async Task<ProductoResponse> CreateAsync(CreateProductoRequest r)
    {
        var p = new Producto
        {
            Id = Guid.NewGuid(),
            Nombre = r.Nombre,
            Slug = r.Slug,
            Descripcion = r.Descripcion,
            Precio = r.Precio,
            CategoriaSlug = r.Categoria,
            Stock = r.Stock,
            Badge = r.Badge,
            Dimensiones = r.Dimensiones,
            Material = r.Material,
            Colores = r.Colores,
            Activo = true,
            Imagenes = (r.Imagenes ?? Array.Empty<string>())
                .Select((url, i) => new ImagenProducto
                {
                    Id = Guid.NewGuid(),
                    Url = url,
                    Orden = i
                }).ToList()
        };

        _db.Productos.Add(p);
        await _db.SaveChangesAsync();
        return Map(p);
    }

    public async Task<ProductoResponse?> UpdateAsync(string slug, UpdateProductoRequest r)
    {
        var p = await _db.Productos.FirstOrDefaultAsync(x => x.Slug == slug);
        if (p is null) return null;

        if (r.Nombre is not null) p.Nombre = r.Nombre;
        if (r.Slug is not null) p.Slug = r.Slug;
        if (r.Descripcion is not null) p.Descripcion = r.Descripcion;
        if (r.Precio.HasValue) p.Precio = r.Precio.Value;
        if (r.Categoria is not null) p.CategoriaSlug = r.Categoria;
        if (r.Stock.HasValue) p.Stock = r.Stock.Value;
        if (r.Badge is not null) p.Badge = r.Badge;
        if (r.Dimensiones is not null) p.Dimensiones = r.Dimensiones;
        if (r.Material is not null) p.Material = r.Material;
        if (r.Colores is not null) p.Colores = r.Colores;
        p.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Map(p);
    }

    public async Task<bool> ToggleAsync(string slug)
    {
        var p = await _db.Productos.FirstOrDefaultAsync(x => x.Slug == slug);
        if (p is null) return false;
        p.Activo = !p.Activo;
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string slug)
    {
        var p = await _db.Productos.FirstOrDefaultAsync(x => x.Slug == slug);
        if (p is null) return false;
        p.Activo = false;
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    private static ProductoResponse Map(Producto p) => new(
        p.Id.ToString(),
        p.Nombre,
        p.Slug,
        p.Descripcion,
        p.Precio,
        p.CategoriaSlug,
        p.Stock,
        p.Badge,
        p.Dimensiones,
        p.Material,
        p.Colores,
        p.Imagenes?.Select(i => i.Url).ToArray() ?? Array.Empty<string>(),
        p.Activo
    );
}
