namespace StockAlert.Domain.Services
{
    public interface IEmailService
    {
        Task SendAlertEmailAsync(string recipientEmail, string subject, string body);
    }
}
