namespace TiendaMuebles.Application.Interfaces;

public interface IImageStorageService
{
    Task<ImageUploadResult> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task<bool> DeleteAsync(string publicId);
}

public record ImageUploadResult(string Url, string PublicId);
