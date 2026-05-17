namespace TiendaMuebles.Application.DTOs;

public record CategoriaResponse(
    string Id,
    string Nombre,
    string Slug,
    int Orden
);

public record CreateCategoriaRequest(
    string Nombre,
    string Slug,
    int Orden
);
