using Microsoft.EntityFrameworkCore;
using TiendaMuebles.Application.DTOs;
using TiendaMuebles.Infrastructure.Data;
using TiendaMuebles.Infrastructure.Services;
using Xunit;

namespace TiendaMuebles.Api.Tests;

public class ProductoServiceTests
{
    private AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private async Task SeedCategoria(AppDbContext db)
    {
        db.Categorias.Add(new Domain.Entities.Categoria
        {
            Id = Guid.NewGuid(), Nombre = "Sala", Slug = "sala", Orden = 1
        });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateAsync_ReturnsProductoMappedCorrectly()
    {
        var db = CreateDb();
        await SeedCategoria(db);
        var svc = new ProductoService(db);

        var result = await svc.CreateAsync(new CreateProductoRequest(
            "Sofa Test", "sofa-test", "Un sofa elegante", 15000m, "sala",
            10, "Nuevo", "200x100 cm", "Tela", new[] { "#000" }, null));

        Assert.Equal("Sofa Test", result.Nombre);
        Assert.Equal("sofa-test", result.Slug);
        Assert.Equal("sala", result.Categoria);
        Assert.Equal(15000m, result.Precio);
        Assert.True(result.Activo);
        Assert.NotNull(result.Id);
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsProducto()
    {
        var db = CreateDb();
        await SeedCategoria(db);
        var svc = new ProductoService(db);
        await svc.CreateAsync(new CreateProductoRequest(
            "Sofa Test", "sofa-test", "Desc", 10000m, "sala", 5, null, null, null, null, null));

        var result = await svc.GetBySlugAsync("sofa-test");

        Assert.NotNull(result);
        Assert.Equal("Sofa Test", result!.Nombre);
    }

    [Fact]
    public async Task GetBySlugAsync_NotFound_ReturnsNull()
    {
        var db = CreateDb();
        var svc = new ProductoService(db);
        var result = await svc.GetBySlugAsync("no-existe");
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProductosAsync_FiltersByCategoria()
    {
        var db = CreateDb();
        await SeedCategoria(db);
        db.Categorias.Add(new Domain.Entities.Categoria { Id = Guid.NewGuid(), Nombre = "Comedor", Slug = "comedor", Orden = 2 });
        await db.SaveChangesAsync();
        var svc = new ProductoService(db);

        await svc.CreateAsync(new CreateProductoRequest("Sofa", "sofa", "d", 10000m, "sala", 1, null, null, null, null, null));
        await svc.CreateAsync(new CreateProductoRequest("Mesa", "mesa", "d", 20000m, "comedor", 1, null, null, null, null, null));

        var result = await svc.GetProductosAsync("sala", 1, 12);

        Assert.Single(result.Data);
        Assert.Equal("Sofa", result.Data[0].Nombre);
    }

    [Fact]
    public async Task ToggleAsync_DeactivatesProduct()
    {
        var db = CreateDb();
        await SeedCategoria(db);
        var svc = new ProductoService(db);
        await svc.CreateAsync(new CreateProductoRequest("Sofa", "sofa", "d", 10000m, "sala", 1, null, null, null, null, null));

        var ok = await svc.ToggleAsync("sofa");
        Assert.True(ok);

        // Al desactivar, GetBySlug (que filtra Activo=true) devuelve null
        var p = await svc.GetBySlugAsync("sofa");
        Assert.Null(p);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesRemovesFromPublicListing()
    {
        var db = CreateDb();
        await SeedCategoria(db);
        var svc = new ProductoService(db);
        await svc.CreateAsync(new CreateProductoRequest("Sofa", "sofa", "d", 10000m, "sala", 1, null, null, null, null, null));

        var ok = await svc.DeleteAsync("sofa");
        Assert.True(ok);

        var result = await svc.GetProductosAsync(null, 1, 12);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetDestacadosAsync_ReturnsOnlyWithBadge()
    {
        var db = CreateDb();
        await SeedCategoria(db);
        var svc = new ProductoService(db);
        await svc.CreateAsync(new CreateProductoRequest("Normal", "normal", "d", 1000m, "sala", 1, null, null, null, null, null));
        await svc.CreateAsync(new CreateProductoRequest("Destacado", "destacado", "d", 2000m, "sala", 5, "Bestseller", null, null, null, null));

        var result = await svc.GetDestacadosAsync();

        Assert.Single(result.Data);
        Assert.Equal("Destacado", result.Data[0].Nombre);
    }

    [Fact]
    public async Task CreateAsync_WithImagenes_MapsToUrls()
    {
        var db = CreateDb();
        await SeedCategoria(db);
        var svc = new ProductoService(db);

        var result = await svc.CreateAsync(new CreateProductoRequest(
            "Sofa", "sofa", "d", 10000m, "sala", 1, null, null, null, null,
            new[] { "/img/foto1.jpg", "/img/foto2.jpg" }));

        Assert.Equal(2, result.Imagenes.Length);
        Assert.Contains("/img/foto1.jpg", result.Imagenes);
    }
}
