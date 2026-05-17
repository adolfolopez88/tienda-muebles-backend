namespace TiendaMuebles.Application.DTOs;

// Respuesta: mapea EXACTAMENTE el Product interface del frontend
// { id, nombre, slug, descripcion, precio, categoria, stock, badge?, dimensiones, material, colores, imagenes, activo }
public record ProductoResponse(
    string Id,
    string Nombre,
    string Slug,
    string Descripcion,
    decimal Precio,
    string Categoria,
    int Stock,
    string? Badge,
    string? Dimensiones,
    string? Material,
    string[]? Colores,
    string[] Imagenes,
    bool Activo
);

// Create: lo que envia el frontend al crear producto (Partial<Product>)
public record CreateProductoRequest(
    string Nombre,
    string Slug,
    string Descripcion,
    decimal Precio,
    string Categoria,
    int Stock,
    string? Badge,
    string? Dimensiones,
    string? Material,
    string[]? Colores,
    string[]? Imagenes
);

// Update: igual que create pero todo opcional
public record UpdateProductoRequest(
    string? Nombre,
    string? Slug,
    string? Descripcion,
    decimal? Precio,
    string? Categoria,
    int? Stock,
    string? Badge,
    string? Dimensiones,
    string? Material,
    string[]? Colores,
    string[]? Imagenes
);

// Paginacion
public record ProductoListResponse(
    List<ProductoResponse> Data,
    PaginationInfo Pagination
);

public record PaginationInfo(
    int Page,
    int Limit,
    int Total,
    int TotalPages
);
