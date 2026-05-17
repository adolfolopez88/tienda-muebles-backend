namespace TiendaMuebles.Domain.Entities;

public class Producto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string CategoriaSlug { get; set; } = string.Empty;
    public int Stock { get; set; } = 10;
    public string? Badge { get; set; }
    public string? Dimensiones { get; set; }
    public string? Material { get; set; }
    public string[]? Colores { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Categoria? Categoria { get; set; }
    public ICollection<ImagenProducto> Imagenes { get; set; } = new List<ImagenProducto>();
}
