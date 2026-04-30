using Microsoft.Extensions.Logging;
using StockAlert.Domain.Services;

namespace StockAlert.Infrastructure.ExternalServices.Email
{
    public class FakeEmailService : IEmailService
    {
        private readonly ILogger<FakeEmailService> _logger;

        public FakeEmailService(ILogger<FakeEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAlertEmailAsync(string recipientEmail, string subject, string body)
        {
            // Simula o envio de e-mail no console do VS
            _logger.LogInformation("\n" + new string('=', 40) +
                                   "\n[FAKE EMAIL SENT]" +
                                   $"\nTo: {recipientEmail}" +
                                   $"\nSubject: {subject}" +            
                                   $"\nBody: {body}\n" +
                                   new string('=', 40));

            return Task.CompletedTask;
        }
    }
}
