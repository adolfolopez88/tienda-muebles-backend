using TiendaMuebles.Application.DTOs;

namespace TiendaMuebles.Application.Interfaces;

public interface ICategoriaService
{
    Task<List<CategoriaResponse>> GetAllAsync();
    Task<CategoriaResponse> CreateAsync(CreateCategoriaRequest request);
}
