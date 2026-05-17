using TiendaMuebles.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace TiendaMuebles.Infrastructure.Services;

public class LoggingEmailService : IEmailService
{
    private readonly ILogger<LoggingEmailService> _logger;

    public LoggingEmailService(ILogger<LoggingEmailService> logger) => _logger = logger;

    public Task SendOrderConfirmationAsync(string toEmail, string orderNumber, decimal total, int itemCount)
    {
        _logger.LogInformation(
            "[EMAIL] To: {To} | Subject: Confirmacion de pedido {Order} | Total: {Total:C} | Items: {Items}",
            toEmail, orderNumber, total, itemCount);
        return Task.CompletedTask;
    }

    public Task SendShippingNotificationAsync(string toEmail, string orderNumber)
    {
        _logger.LogInformation(
            "[EMAIL] To: {To} | Subject: Pedido {Order} enviado",
            toEmail, orderNumber);
        return Task.CompletedTask;
    }
}
