using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using StockAlert.Domain.Services;

namespace StockAlert.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAlertEmailAsync(string recipientEmail, string subject, string body)
    {
        //configurações do appsettings.json
        var host = _configuration["EmailSettings:Host"];
        var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
        var userName = _configuration["EmailSettings:UserName"];
        var password = _configuration["EmailSettings:Password"];

        // 2. Configuramos o "cliente" que vai se conectar ao servidor
        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(userName, password),
            EnableSsl = true // Garante que a conexão seja segura
        };

        // 3. Criamos a mensagem de e-mail
        var mailMessage = new MailMessage
        {
            From = new MailAddress(userName!),
            Subject = subject,
            Body = body,
            IsBodyHtml = false // Definimos como texto simples por enquanto
        };
        mailMessage.To.Add(recipientEmail);

        // 4. O carteiro (SMTP) entrega a mensagem
        await client.SendMailAsync(mailMessage);
    }
}
