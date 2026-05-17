namespace TiendaMuebles.Domain.Entities;

public class Categoria
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int Orden { get; set; }
}
