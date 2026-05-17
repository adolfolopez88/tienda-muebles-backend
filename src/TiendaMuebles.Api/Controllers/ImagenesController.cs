using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaMuebles.Application.Interfaces;
using TiendaMuebles.Infrastructure.Data;
using TiendaMuebles.Domain.Entities;

namespace TiendaMuebles.Api.Controllers;

[ApiController]
[Route("api/imagenes")]
public class ImagenesController : ControllerBase
{
    private readonly IImageStorageService _storage;
    private readonly AppDbContext _db;

    public ImagenesController(IImageStorageService storage, AppDbContext db)
    {
        _storage = storage;
        _db = db;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Upload(
        [FromForm] IFormFile file,
        [FromForm] Guid productoId,
        [FromForm] string? alt)
    {
        try
        {
            using var stream = file.OpenReadStream();
            var result = await _storage.UploadAsync(stream, file.FileName, file.ContentType);

            var imagen = new ImagenProducto
            {
                Id = Guid.NewGuid(),
                ProductoId = productoId,
                Url = result.Url,
                Alt = alt,
                Orden = await _db.ImagenProductos.CountAsync(i => i.ProductoId == productoId)
            };

            _db.ImagenProductos.Add(imagen);
            await _db.SaveChangesAsync();

            return Ok(new { id = imagen.Id.ToString(), url = imagen.Url });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var imagen = await _db.ImagenProductos.FindAsync(id);
        if (imagen is null) return NotFound();

        // Extract publicId from URL: /uploads/filename.jpg -> filename.jpg
        var publicId = Path.GetFileName(imagen.Url);
        await _storage.DeleteAsync(publicId);

        _db.ImagenProductos.Remove(imagen);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
