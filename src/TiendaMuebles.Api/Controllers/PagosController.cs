using Microsoft.AspNetCore.Mvc;
using TiendaMuebles.Application.Interfaces;

namespace TiendaMuebles.Api.Controllers;

[ApiController]
[Route("api/pagos")]
public class PagosController : ControllerBase
{
    private readonly IPaymentService _payment;

    public PagosController(IPaymentService payment) => _payment = payment;

    public record CreatePaymentIntentRequest(string PedidoNumero, decimal Amount);

    [HttpPost("create-intent")]
    public async Task<ActionResult> CreateIntent(CreatePaymentIntentRequest r)
    {
        var result = await _payment.CreatePaymentIntentAsync(r.Amount, r.PedidoNumero);
        return Ok(new { clientSecret = result.ClientSecret });
    }

    [HttpPost("webhook")]
    public async Task<ActionResult> Webhook()
    {
        // En produccion: verificar Stripe-Signature y actualizar pedido
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        await _payment.ConfirmPaymentAsync(json);
        return Ok();
    }
}
