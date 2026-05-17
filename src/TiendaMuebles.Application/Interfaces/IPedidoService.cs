using TiendaMuebles.Application.DTOs;

namespace TiendaMuebles.Application.Interfaces;

public interface IPedidoService
{
    Task<PedidoResponse> CreateAsync(CreatePedidoRequest request);
    Task<PedidoResponse?> GetByNumeroAsync(string numero);
    Task<PedidoListResponse> GetAllAsync(string? estado, int page = 1, int limit = 12);
    Task<PedidoResponse?> UpdateEstadoAsync(string numero, string estado);
}
