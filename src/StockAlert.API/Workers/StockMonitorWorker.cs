using Microsoft.Extensions.Options;
using StockAlert.API.Configurations;
using StockAlert.Domain.Repositories;
using StockAlert.Domain.Services;

namespace StockAlert.API.Workers;

public class StockMonitorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StockMonitorWorker> _logger;
    private readonly WorkerSettings _settings;

    public StockMonitorWorker(
        IServiceProvider serviceProvider,
        ILogger<StockMonitorWorker> logger,
        IOptions<WorkerSettings> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock Monitor Worker is starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var alertRepo = scope.ServiceProvider.GetRequiredService<IAlertRuleRepository>();
                var brapiService = scope.ServiceProvider.GetRequiredService<IBrapiService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var historyRepo = scope.ServiceProvider.GetRequiredService<INotificationHistoryRepository>();
                var conditionChecker = scope.ServiceProvider.GetRequiredService<IAlertConditionChecker>();

                var activeRules = await alertRepo.GetAllActiveAsync();

                foreach (var rule in activeRules)
                {
                    var quote = await brapiService.GetStockQuoteAsync(rule.StockSymbol);

                    if (quote is null)
                        continue;

                    if (!conditionChecker.IsConditionMet(quote.Price, quote.PreviousClose, rule))
                        continue;

                    if (!CanSendNotification(rule))
                        continue;

                    _logger.LogInformation(
                        "Condition met for {StockSymbol} (User: {Email})",
                        rule.StockSymbol,
                        rule.User?.Email
                    );

                    var subject = $"Alerta de ação: {rule.StockSymbol} atingiu sua condição";

                    var message = BuildEmailMessage(rule, quote.Price);

                    await emailService.SendAlertEmailAsync(
                        rule.User!.Email,
                        subject,
                        message
                    );

                    var history = new StockAlert.Domain.Entities.NotificationHistory
                    {
                        AlertRuleId = rule.Id,
                        UserId = rule.UserId,
                        SentAt = DateTime.UtcNow,
                        Channel = StockAlert.Domain.Enums.NotificationChannel.Email,
                        Success = true,
                        Status = "Sent",
                        Message = message,
                        Recipient = rule.User!.Email
                    };

                    await historyRepo.AddAsync(history);

                    rule.LastTriggeredAt = DateTime.UtcNow;

                    if (rule.NotifyOnce)
                        rule.IsActive = false;

                    await alertRepo.UpdateAsync(rule);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Stock Monitor Worker.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_settings.IntervalSeconds), stoppingToken);
        }
    }

    private static string BuildEmailMessage(StockAlert.Domain.Entities.AlertRule rule, decimal currentPrice)
    {
        var targetInfo = rule.TargetPrice.HasValue
            ? $"Preço alvo: R$ {rule.TargetPrice.Value:F2}"
            : $"Variação alvo: {rule.PercentageChange:F2}%";

        return $"""
        Olá, {rule.User!.Name}!

        A ação {rule.StockSymbol} atingiu a condição configurada no seu alerta.

        Preço atual: R$ {currentPrice:F2}
        {targetInfo}
        Condição: {rule.Operator}

        Data do alerta: {DateTime.Now:dd/MM/yyyy HH:mm}

        Este alerta foi enviado automaticamente pelo StockAlert.
        """;
    }

    private static bool CanSendNotification(StockAlert.Domain.Entities.AlertRule rule)
    {
        if (rule.LastTriggeredAt is null)
            return true;

        var cooldown = rule.CooldownMinutes ?? 15;

        return DateTime.UtcNow >= rule.LastTriggeredAt.Value.AddMinutes(cooldown);
    }
}