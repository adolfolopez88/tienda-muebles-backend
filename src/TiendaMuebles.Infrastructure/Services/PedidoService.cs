using Microsoft.EntityFrameworkCore;
using TiendaMuebles.Application.DTOs;
using TiendaMuebles.Application.Interfaces;
using TiendaMuebles.Domain.Entities;
using TiendaMuebles.Domain.Enums;
using TiendaMuebles.Infrastructure.Data;

namespace TiendaMuebles.Infrastructure.Services;

public class PedidoService : IPedidoService
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;

    public PedidoService(AppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task<PedidoResponse> CreateAsync(CreatePedidoRequest r)
    {
        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            Numero = await GenerateOrderNumberAsync(),
            Email = r.DatosEnvio.Email,
            MetodoPago = r.MetodoPago,
            Subtotal = r.Subtotal,
            Estado = EstadoPedido.Pendiente,
            Items = r.Items.Select(i => new ItemPedido
            {
                Id = Guid.NewGuid(),
                Producto = i.Producto,
                Nombre = i.Nombre,
                Color = i.Color,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario
            }).ToList(),
            DatosEnvio = new DatosEnvio
            {
                Id = Guid.NewGuid(),
                Nombre = r.DatosEnvio.Nombre,
                Apellido = r.DatosEnvio.Apellido,
                Email = r.DatosEnvio.Email,
                Telefono = r.DatosEnvio.Telefono,
                Direccion = r.DatosEnvio.Direccion,
                Ciudad = r.DatosEnvio.Ciudad,
                CP = r.DatosEnvio.CP,
                Estado = r.DatosEnvio.Estado
            }
        };

        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync();

        // Disparar email de confirmacion (no bloqueante)
        _ = _email.SendOrderConfirmationAsync(
            pedido.Email, pedido.Numero, pedido.Subtotal, pedido.Items.Count);

        return Map(pedido);
    }

    public async Task<PedidoResponse?> GetByNumeroAsync(string numero)
    {
        var p = await _db.Pedidos
            .Include(x => x.Items)
            .Include(x => x.DatosEnvio)
            .FirstOrDefaultAsync(x => x.Numero == numero);

        return p is null ? null : Map(p);
    }

    public async Task<PedidoListResponse> GetAllAsync(string? estado, int page = 1, int limit = 12)
    {
        var query = _db.Pedidos
            .Include(x => x.Items)
            .Include(x => x.DatosEnvio)
            .AsQueryable();

        if (!string.IsNullOrEmpty(estado) && Enum.TryParse<EstadoPedido>(estado, true, out var e))
            query = query.Where(p => p.Estado == e);

        var total = await query.CountAsync();
        var pedidos = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return new PedidoListResponse(
            pedidos.Select(Map).ToList(),
            new PaginationInfo(page, limit, total, (int)Math.Ceiling((double)total / limit))
        );
    }

    public async Task<PedidoResponse?> UpdateEstadoAsync(string numero, string estado)
    {
        if (!Enum.TryParse<EstadoPedido>(estado, true, out var nuevoEstado))
            return null;

        var p = await _db.Pedidos
            .Include(x => x.Items)
            .Include(x => x.DatosEnvio)
            .FirstOrDefaultAsync(x => x.Numero == numero);

        if (p is null) return null;

        p.Estado = nuevoEstado;
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        if (nuevoEstado == EstadoPedido.Enviado)
            _ = _email.SendShippingNotificationAsync(p.Email, p.Numero);

        return Map(p);
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        string numero;
        do
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var suffix = new string(Enumerable.Range(0, 5)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
            numero = $"MN-{suffix}";
        }
        while (await _db.Pedidos.AnyAsync(p => p.Numero == numero));

        return numero;
    }

    private static PedidoResponse Map(Pedido p) => new(
        p.Numero,
        p.Email,
        p.Estado.ToString(),
        p.MetodoPago,
        p.Subtotal,
        p.Items.Select(i => new ItemPedidoResponse(
            i.Producto, i.Nombre, i.Color, i.Cantidad, i.PrecioUnitario)).ToList(),
        new DatosEnvioResponse(
            p.DatosEnvio.Nombre, p.DatosEnvio.Apellido, p.DatosEnvio.Email,
            p.DatosEnvio.Telefono, p.DatosEnvio.Direccion, p.DatosEnvio.Ciudad,
            p.DatosEnvio.CP, p.DatosEnvio.Estado),
        p.CreatedAt
    );
}
