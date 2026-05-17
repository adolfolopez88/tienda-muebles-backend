using TiendaMuebles.Domain.Enums;

namespace TiendaMuebles.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public Guid? ClienteId { get; set; }
    public string Email { get; set; } = string.Empty;
    public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;
    public string MetodoPago { get; set; } = "stripe";
    public decimal Subtotal { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ItemPedido> Items { get; set; } = new List<ItemPedido>();
    public DatosEnvio DatosEnvio { get; set; } = new();
}

public class ItemPedido
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public string Producto { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public Pedido? Pedido { get; set; }
}

public class DatosEnvio
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string CP { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public Pedido? Pedido { get; set; }
}
