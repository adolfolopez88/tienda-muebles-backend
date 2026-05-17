using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TiendaMuebles.Application.DTOs;
using TiendaMuebles.Application.Interfaces;
using TiendaMuebles.Infrastructure.Data;
using TiendaMuebles.Infrastructure.Services;
using Xunit;

namespace TiendaMuebles.Api.Tests;

public class PedidoServiceTests
{
    private AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private IEmailService CreateEmailMock() => new Mock<IEmailService>().Object;

    private CreatePedidoRequest SampleRequest() => new(
        new()
        {
            new("sofa-capitone", "Sofa Capitone", "#1a1a1a", 1, 28500m)
        },
        new DatosEnvioRequest("Juan", "Perez", "juan@test.com", "5551234567",
            "Av. Reforma 100", "CDMX", "06600", "CDMX"),
        "stripe",
        28500m
    );

    [Fact]
    public async Task CreateAsync_GeneratesOrderNumber()
    {
        var db = CreateDb();
        var svc = new PedidoService(db, CreateEmailMock());
        var result = await svc.CreateAsync(SampleRequest());

        Assert.StartsWith("MN-", result.Numero);
        Assert.Equal(8, result.Numero.Length);
        Assert.Equal("juan@test.com", result.Email);
        Assert.Equal("Pendiente", result.Estado);
        Assert.Equal(28500m, result.Subtotal);
    }

    [Fact]
    public async Task CreateAsync_SavesItemsAsSnapshots()
    {
        var db = CreateDb();
        var svc = new PedidoService(db, CreateEmailMock());
        var result = await svc.CreateAsync(SampleRequest());

        Assert.Single(result.Items);
        Assert.Equal("Sofa Capitone", result.Items[0].Nombre);
        Assert.Equal("sofa-capitone", result.Items[0].Producto);
        Assert.Equal(28500m, result.Items[0].PrecioUnitario);
    }

    [Fact]
    public async Task CreateAsync_SavesShippingData()
    {
        var db = CreateDb();
        var svc = new PedidoService(db, CreateEmailMock());
        var result = await svc.CreateAsync(SampleRequest());

        Assert.Equal("Juan", result.DatosEnvio.Nombre);
        Assert.Equal("Perez", result.DatosEnvio.Apellido);
        Assert.Equal("CDMX", result.DatosEnvio.Estado);
    }

    [Fact]
    public async Task GetByNumeroAsync_FindsOrder()
    {
        var db = CreateDb();
        var svc = new PedidoService(db, CreateEmailMock());
        var created = await svc.CreateAsync(SampleRequest());
        var found = await svc.GetByNumeroAsync(created.Numero);

        Assert.NotNull(found);
        Assert.Equal(created.Numero, found!.Numero);
    }

    [Fact]
    public async Task GetByNumeroAsync_NotFound_ReturnsNull()
    {
        var db = CreateDb();
        var svc = new PedidoService(db, CreateEmailMock());
        Assert.Null(await svc.GetByNumeroAsync("MN-XXXXX"));
    }

    [Fact]
    public async Task GetAllAsync_ListsOrders()
    {
        var db = CreateDb();
        var svc = new PedidoService(db, CreateEmailMock());
        await svc.CreateAsync(SampleRequest());

        var result = await svc.GetAllAsync(null, 1, 12);
        Assert.NotEmpty(result.Data);
        Assert.Equal(1, result.Pagination.Total);
    }

    [Fact]
    public async Task UpdateEstadoAsync_ChangesStatus()
    {
        var db = CreateDb();
        var svc = new PedidoService(db, CreateEmailMock());
        var created = await svc.CreateAsync(SampleRequest());
        var updated = await svc.UpdateEstadoAsync(created.Numero, "Enviado");

        Assert.NotNull(updated);
        Assert.Equal("Enviado", updated!.Estado);
    }

    [Fact]
    public async Task UpdateEstadoAsync_InvalidState_ReturnsNull()
    {
        var db = CreateDb();
        var svc = new PedidoService(db, CreateEmailMock());
        var created = await svc.CreateAsync(SampleRequest());

        Assert.Null(await svc.UpdateEstadoAsync(created.Numero, "EstadoInventado"));
    }

    [Fact]
    public async Task CreateAsync_SendsConfirmationEmail()
    {
        var db = CreateDb();
        var mock = new Mock<IEmailService>();
        var svc = new PedidoService(db, mock.Object);
        var result = await svc.CreateAsync(SampleRequest());

        mock.Verify(e => e.SendOrderConfirmationAsync(
            "juan@test.com", result.Numero, 28500m, 1), Times.Once);
    }

    [Fact]
    public async Task UpdateEstadoAsync_ToEnviado_SendsShippingEmail()
    {
        var db = CreateDb();
        var mock = new Mock<IEmailService>();
        var svc = new PedidoService(db, mock.Object);
        var created = await svc.CreateAsync(SampleRequest());

        await svc.UpdateEstadoAsync(created.Numero, "Enviado");

        mock.Verify(e => e.SendShippingNotificationAsync(
            "juan@test.com", created.Numero), Times.Once);
    }
}
