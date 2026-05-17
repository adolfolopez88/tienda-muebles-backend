namespace TiendaMuebles.Domain.Entities;

public class ImagenProducto
{
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Alt { get; set; }
    public int Orden { get; set; }

    public Producto? Producto { get; set; }
}
