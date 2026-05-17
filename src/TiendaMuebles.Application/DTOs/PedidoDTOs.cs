namespace TiendaMuebles.Application.DTOs;

// Request: exactamente lo que envia el frontend como OrderPayload
public record CreatePedidoRequest(
    List<ItemPedidoRequest> Items,
    DatosEnvioRequest DatosEnvio,
    string MetodoPago,
    decimal Subtotal
);

public record ItemPedidoRequest(
    string Producto,
    string Nombre,
    string Color,
    int Cantidad,
    decimal PrecioUnitario
);

public record DatosEnvioRequest(
    string Nombre,
    string Apellido,
    string Email,
    string Telefono,
    string Direccion,
    string Ciudad,
    string CP,
    string Estado
);

// Response: mapea EXACTAMENTE el Order interface del frontend
public record PedidoResponse(
    string Numero,
    string Email,
    string Estado,
    string MetodoPago,
    decimal Subtotal,
    List<ItemPedidoResponse> Items,
    DatosEnvioResponse DatosEnvio,
    DateTime CreatedAt
);

public record ItemPedidoResponse(
    string Producto,
    string Nombre,
    string Color,
    int Cantidad,
    decimal PrecioUnitario
);

public record DatosEnvioResponse(
    string Nombre,
    string Apellido,
    string Email,
    string Telefono,
    string Direccion,
    string Ciudad,
    string CP,
    string Estado
);

public record PedidoListResponse(
    List<PedidoResponse> Data,
    PaginationInfo Pagination
);

public record UpdateEstadoRequest(string Estado);
