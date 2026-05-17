namespace TiendaMuebles.Application.Interfaces;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(string toEmail, string orderNumber, decimal total, int itemCount);
    Task SendShippingNotificationAsync(string toEmail, string orderNumber);
}
