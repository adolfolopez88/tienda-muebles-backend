using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaMuebles.Domain.Entities;
using TiendaMuebles.Infrastructure.Auth;
using TiendaMuebles.Infrastructure.Data;

namespace TiendaMuebles.Api.Controllers;

[ApiController]
[Route("api/seed")]
public class SeedController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher _hasher;

    public SeedController(AppDbContext db, PasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Seed()
    {
        // Limpiar existente
        _db.ImagenProductos.RemoveRange(_db.ImagenProductos);
        _db.Productos.RemoveRange(_db.Productos);
        _db.Categorias.RemoveRange(_db.Categorias);
        await _db.SaveChangesAsync();

        // Categorias
        var categorias = new[]
        {
            new Categoria { Id = Guid.NewGuid(), Nombre = "Sala", Slug = "sala", Orden = 1 },
            new Categoria { Id = Guid.NewGuid(), Nombre = "Comedor", Slug = "comedor", Orden = 2 },
            new Categoria { Id = Guid.NewGuid(), Nombre = "Dormitorio", Slug = "dormitorio", Orden = 3 },
            new Categoria { Id = Guid.NewGuid(), Nombre = "Oficina", Slug = "oficina", Orden = 4 }
        };
        _db.Categorias.AddRange(categorias);
        await _db.SaveChangesAsync();

        // Productos (8, mismos del seed MAISON NOIR)
        var productos = new[]
        {
            new Producto { Id = Guid.NewGuid(), Nombre = "Sofa Capitone", Slug = "sofa-capitone", Descripcion = "Sofa de lujo tapizado en terciopelo con capitone artesanal.", Precio = 28500m, CategoriaSlug = "sala", Stock = 5, Badge = "Bestseller", Dimensiones = "220 x 95 x 85 cm", Material = "Terciopelo + Madera de roble", Colores = new[] { "#1a1a1a", "#c4a46c", "#2f3e46" } },
            new Producto { Id = Guid.NewGuid(), Nombre = "Sillon Arcon", Slug = "sillon-arcon", Descripcion = "Sillon individual con reposabrazos curvos de estilo art deco.", Precio = 14200m, CategoriaSlug = "sala", Stock = 8, Badge = null, Dimensiones = "85 x 80 x 90 cm", Material = "Lino + Estructura de nogal", Colores = new[] { "#f5ebe0", "#5c6b73" } },
            new Producto { Id = Guid.NewGuid(), Nombre = "Mesa Recolte", Slug = "mesa-recolte", Descripcion = "Mesa de comedor extensible con acabado en nogal satinado.", Precio = 22800m, CategoriaSlug = "comedor", Stock = 3, Badge = "Nuevo", Dimensiones = "180-240 x 100 x 76 cm", Material = "Nogal macizo", Colores = new[] { "#5c3a21", "#3b2816" } },
            new Producto { Id = Guid.NewGuid(), Nombre = "Silla Bistro Noir", Slug = "silla-bistro-noir", Descripcion = "Silla de comedor estilo bistrot frances, estructura de hierro forjado.", Precio = 6400m, CategoriaSlug = "comedor", Stock = 20, Badge = null, Dimensiones = "45 x 50 x 92 cm", Material = "Hierro forjado + Asiento de rattan", Colores = new[] { "#1a1a1a", "#8b7355" } },
            new Producto { Id = Guid.NewGuid(), Nombre = "Cama Arc", Slug = "cama-arc", Descripcion = "Cama king size con cabecero arqueado tapizado en lino premium.", Precio = 31500m, CategoriaSlug = "dormitorio", Stock = 2, Badge = "Exclusivo", Dimensiones = "210 x 230 x 120 cm", Material = "Lino premium + Roble frances", Colores = new[] { "#e8dcc8", "#4a4a4a" } },
            new Producto { Id = Guid.NewGuid(), Nombre = "Mesita Ombre", Slug = "mesita-ombre", Descripcion = "Mesita de noche con acabado ombre lacado a mano.", Precio = 8900m, CategoriaSlug = "dormitorio", Stock = 12, Badge = null, Dimensiones = "50 x 40 x 55 cm", Material = "MDF lacado + Patas de metal dorado", Colores = new[] { "#d4a574", "#2c2c2c" } },
            new Producto { Id = Guid.NewGuid(), Nombre = "Escritorio Voltaire", Slug = "escritorio-voltaire", Descripcion = "Escritorio ejecutivo con cajoneria oculta y superficie de cuero.", Precio = 18600m, CategoriaSlug = "oficina", Stock = 6, Badge = "Bestseller", Dimensiones = "150 x 70 x 76 cm", Material = "Cuero + Nogal + Detalles de bronce", Colores = new[] { "#3b2816", "#1a1a1a" } },
            new Producto { Id = Guid.NewGuid(), Nombre = "Silla Directeur", Slug = "silla-directeur", Descripcion = "Silla de escritorio ergonomica con tapizado en cuero genuino.", Precio = 11200m, CategoriaSlug = "oficina", Stock = 10, Badge = null, Dimensiones = "62 x 58 x 98 cm", Material = "Cuero genuino + Aluminio pulido", Colores = new[] { "#1a1a1a", "#8b4513" } }
        };
        _db.Productos.AddRange(productos);
        await _db.SaveChangesAsync();

        // Admin default (solo si no existe)
        if (!await _db.Users.AnyAsync(u => u.Email == "admin@maisonnoir.mx"))
        {
            _db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@maisonnoir.mx",
                PasswordHash = _hasher.Hash("admin123"),
                Role = Domain.Enums.UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }

        return Ok(new { message = "Seed completado: 4 categorias, 8 productos, 1 admin" });
    }
}
