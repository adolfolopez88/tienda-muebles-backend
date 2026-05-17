namespace TiendaMuebles.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string pedidoNumero);
    Task<bool> ConfirmPaymentAsync(string paymentIntentId);
}

public record PaymentIntentResult(string ClientSecret, string PaymentIntentId);
