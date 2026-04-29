using StockAlert.Domain.Repositories;
using StockAlert.Domain.Services;

namespace StockAlert.API.Workers;

public class StockMonitorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StockMonitorWorker> _logger;

    public StockMonitorWorker(IServiceProvider serviceProvider, ILogger<StockMonitorWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
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

                    var conditionMet = conditionChecker.IsConditionMet(
                        quote.Price,
                        quote.PreviousClose,
                        rule
                    );

                    if (!conditionMet)
                        continue;

                    if (!CanSendNotification(rule))
                        continue;

                    _logger.LogInformation(
                        "Condition met for {StockSymbol} (User: {Email})",
                        rule.StockSymbol,
                        rule.User?.Email
                    );

                    await emailService.SendAlertEmailAsync(
                        rule.User!.Email,
                        $"Stock Alert: {rule.StockSymbol} reached target!",
                        $"The stock {rule.StockSymbol} is now {quote.Price:C2}."
                    );

                    var history = new StockAlert.Domain.Entities.NotificationHistory
                    {
                        AlertRuleId = rule.Id,
                        UserId = rule.UserId,
                        SentAt = DateTime.UtcNow,
                        Channel = StockAlert.Domain.Enums.NotificationChannel.Email,
                        Success = true,
                        Status = "Sent",
                        Message = $"Stock {rule.StockSymbol} reached {quote.Price:C2}",
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

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private static bool CanSendNotification(StockAlert.Domain.Entities.AlertRule rule)
    {
        if (rule.LastTriggeredAt is null)
            return true;

        var cooldown = rule.CooldownMinutes ?? 15;

        return DateTime.UtcNow >= rule.LastTriggeredAt.Value.AddMinutes(cooldown);
    }
}