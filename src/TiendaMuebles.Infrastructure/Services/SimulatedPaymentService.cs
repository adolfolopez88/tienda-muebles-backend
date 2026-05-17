using TiendaMuebles.Application.Interfaces;

namespace TiendaMuebles.Infrastructure.Services;

public class SimulatedPaymentService : IPaymentService
{
    public Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string pedidoNumero)
    {
        // En produccion: Stripe PaymentIntent.CreateAsync()
        var paymentIntentId = $"pi_sim_{Guid.NewGuid().ToString("N")[..16]}";
        var clientSecret = $"{paymentIntentId}_secret_sim";
        return Task.FromResult(new PaymentIntentResult(clientSecret, paymentIntentId));
    }

    public Task<bool> ConfirmPaymentAsync(string paymentIntentId)
    {
        // En produccion: verificar evento webhook de Stripe
        return Task.FromResult(true);
    }
}
