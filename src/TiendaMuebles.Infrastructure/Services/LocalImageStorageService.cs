using TiendaMuebles.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace TiendaMuebles.Infrastructure.Services;

public class LocalImageStorageService : IImageStorageService
{
    private readonly string _uploadPath;
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public LocalImageStorageService(IConfiguration configuration)
    {
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(_uploadPath);
    }

    public async Task<ImageUploadResult> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        if (fileStream.Length > MaxFileSize)
            throw new InvalidOperationException("El archivo excede el limite de 5MB");

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".avif" };
        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException("Formato no permitido. Usar: JPG, PNG, WebP, AVIF");

        var publicId = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadPath, publicId);

        using (var fileStream2 = File.Create(filePath))
        {
            await fileStream.CopyToAsync(fileStream2);
        }

        var url = $"/uploads/{publicId}";
        return new ImageUploadResult(url, publicId);
    }

    public Task<bool> DeleteAsync(string publicId)
    {
        var filePath = Path.Combine(_uploadPath, publicId);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
