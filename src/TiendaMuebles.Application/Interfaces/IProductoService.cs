using TiendaMuebles.Application.DTOs;

namespace TiendaMuebles.Application.Interfaces;

public interface IProductoService
{
    Task<ProductoListResponse> GetProductosAsync(string? categoria, int page = 1, int limit = 12);
    Task<ProductoListResponse> GetDestacadosAsync();
    Task<ProductoResponse?> GetBySlugAsync(string slug);
    Task<ProductoResponse> CreateAsync(CreateProductoRequest request);
    Task<ProductoResponse?> UpdateAsync(string slug, UpdateProductoRequest request);
    Task<bool> ToggleAsync(string slug);
    Task<bool> DeleteAsync(string slug);
}
